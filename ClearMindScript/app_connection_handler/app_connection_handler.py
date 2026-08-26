import json
import logging
import os
import socket
import threading

# Path to the file the engine's port and status are written to, so the WPF app can find and read them
ENGINE_STATUS_FILE_PATH = os.path.join(os.getenv("APPDATA"), "ClearMind", "engine_status.json")

logger = logging.getLogger(__name__)

# Event flag for the start button in the WPF app
wake_event = threading.Event()

_engine_port: int | None = None


def write_engine_status(status: str) -> None:
    with open(ENGINE_STATUS_FILE_PATH, "w") as f:
        json.dump({"port": _engine_port, "status": status}, f)


# Thread function used to force a wakeup by calling wake_event.set()
def listen_for_wakeups() -> None:
    global _engine_port

    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.bind(("127.0.0.1", 0))
    server.listen()

    _engine_port = server.getsockname()[1]
    write_engine_status("SLEEPING")

    logger.info(f"Wakeup listener bound to port {_engine_port}")

    while True:
        conn, _ = server.accept() # Thread is blocked here until the user presses start on the WPF app
        conn.close()
        logger.info("Received wakeup ping")
        wake_event.set()
