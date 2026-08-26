from __future__ import annotations
import json
import os
from json_to_obj_classes.lockedProcessEntry import LockedProcessEntry


# Function that's used to convert raw data from the JSON into usable classes
def load_processes(file_path: str) -> list[LockedProcessEntry]:
    # No processes have been added yet (e.g. right after install), treat as an empty list instead of failing
    if not os.path.exists(file_path):
        return []

    with open(file_path, "r") as f:
        raw_processes = json.load(f)

    processes = []
    for raw_process in raw_processes:
        processes.append(LockedProcessEntry.from_dict(raw_process))

    return processes
