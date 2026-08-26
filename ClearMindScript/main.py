import logging
import os
import threading
import time
from logging.handlers import TimedRotatingFileHandler

from app_connection_handler.app_connection_handler import listen_for_wakeups, wake_event, write_engine_status
from json_to_obj_converter.converter import load_processes
from process_find_and_kill.process_finder_killer import find_active, kill_scheduled_matches
from time_handler.time_handler import (
    find_boundary_time,
    get_current_time,
    today_code,
    seconds_until_time,
    MIDNIGHT,
)

# Path to the JSON file the WPF app writes to, same location on any Windows account
PROCESSES_FILE_PATH = os.path.join(os.getenv("APPDATA"), "ClearMind", "processes.json")
# Path to the log directory where the script logs, same location on any Windows account
LOGS_DIR = os.path.join(os.getenv("APPDATA"), "ClearMind", "logs")

POLL_INTERVAL_SECONDS = 5

os.makedirs(LOGS_DIR, exist_ok=True)

log_handler = TimedRotatingFileHandler(
    os.path.join(LOGS_DIR, "clearmind.log"),
    when="midnight",
    backupCount=30, # Caps the max amount of logs to 30
)
logging.basicConfig(level=logging.INFO, handlers=[log_handler], format="%(asctime)s %(levelname)s %(message)s")
logger = logging.getLogger(__name__)


def run() -> None:
    logger.info("ClearMind engine started")

    threading.Thread(target=listen_for_wakeups, daemon=True).start()

    while True:
        locked_process_entries = load_processes(PROCESSES_FILE_PATH)
        latest_proc_time = find_boundary_time(locked_process_entries, latest=True)
        earliest_start_time = find_boundary_time(locked_process_entries, latest=False)
        current_time = get_current_time()

        if latest_proc_time is None:
            logger.info("No schedules active today, sleeping until midnight")
        else:
            # Nothing to enforce yet today, sleep until the first schedule starts instead of polling
            if current_time < earliest_start_time:
                logger.info(f"First schedule starts at {earliest_start_time}, sleeping until then")
                write_engine_status("SLEEPING")
                wake_event.wait(timeout=seconds_until_time(earliest_start_time))
                wake_event.clear()
                # Re-check from the top instead of falling through: a wakeup ping means the user may have
                # just added or changed a schedule, so the boundary times computed above could be stale.
                continue

            logger.info(f"Enforcing today until {latest_proc_time}")

        enforcing = latest_proc_time is not None and current_time < latest_proc_time
        while enforcing:

            write_engine_status("RUNNING") # Sets the engine status to lock the start button in the app
            today = today_code()

            matches = find_active(locked_process_entries)

            # Ensures we only kill if we found matches
            if matches:
                logger.info(f"Found {len(matches)} matching process(es), checking schedules")
                kill_scheduled_matches(matches, locked_process_entries, current_time, today)

            time.sleep(POLL_INTERVAL_SECONDS)

            # Update the list of locked processes and times incase the user changed the JSON
            locked_process_entries = load_processes(PROCESSES_FILE_PATH)
            latest_proc_time = find_boundary_time(locked_process_entries, latest=True)
            current_time = get_current_time()
            enforcing = latest_proc_time is not None and current_time < latest_proc_time

        # Skipping the current day incase the last time period has passed or
        # there are no process to lock for the day.
        logger.info("Sleeping until midnight")
        write_engine_status("SLEEPING")
        wake_event.wait(timeout=seconds_until_time(MIDNIGHT))
        wake_event.clear()


if __name__ == '__main__':
    run()
