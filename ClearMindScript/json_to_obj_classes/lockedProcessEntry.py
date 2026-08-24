from __future__ import annotations
from dataclasses import dataclass
from json_to_obj_classes.scheduleEntry import ScheduleEntry

# Represents a process and its associated locking schedules
@dataclass
class LockedProcessEntry:
    Id: str
    Name: str
    Schedules: list[ScheduleEntry]

    # Used to populate the fields and create an instance of
    # a LockedProcessEntry when loading the JSON file
    @classmethod
    def from_dict(cls, data: dict) -> "LockedProcessEntry":
        return cls(
            Id=data["Id"],
            Name=data["Name"],
            Schedules=[ScheduleEntry.from_dict(s) for s in data["Schedules"]],
        )