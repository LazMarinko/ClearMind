from __future__ import annotations
import logging
import psutil
from datetime import time

from json_to_obj_classes.lockedProcessEntry import LockedProcessEntry

logger = logging.getLogger(__name__)


# Used to force given string to lower case and strip .exe suffix
def normalize_name(name: str) -> str:
    return name.strip().lower().removesuffix(".exe")


# Used to find currently running processes that match a locked process name
def find_active(locked_processes: list[LockedProcessEntry]) -> list[psutil.Process]:
    locked_names = {normalize_name(p.Name) for p in locked_processes}

    matches = []
    for proc in psutil.process_iter(["name"]): # Iterate over the whole ptable
        proc_name = proc.info["name"]
        if proc_name and normalize_name(proc_name) in locked_names:
            matches.append(proc)

    return matches


# Used to kill matched processes whose schedule is currently active
def kill_scheduled_matches(
        matches: list[psutil.Process],
        locked_processes: list[LockedProcessEntry],
        current_time: time,
        today: str,
) -> None:
    for proc in matches:
        proc_name = normalize_name(proc.info["name"])

        for entry in locked_processes:
            if normalize_name(entry.Name) != proc_name:
                continue

            # Tries to any valid schedule for the current time and date for the current entry
            is_scheduled_now = any(
                today in schedule.ActiveDays and schedule.StartTime <= current_time <= schedule.EndTime
                for schedule in entry.Schedules
            )

            # If a valid schedule is found for the current entry we kill it and break
            if is_scheduled_now:
                proc.kill()
                logger.info(f"Killed {proc.info['name']} (PID {proc.pid})")
                break
