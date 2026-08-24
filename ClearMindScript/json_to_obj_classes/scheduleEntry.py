from __future__ import annotations
from dataclasses import dataclass
from datetime import time

# Represents a Schedule that is created inside the UI part of the app
@dataclass
class ScheduleEntry:
    Id: str
    ActiveDays: list[str]
    StartTime: time
    EndTime: time

    # Used to populate the fields and create an instance of
    # a ScheduleEntry class.
    # Called when a LockedProcessEntry is being made.
    @classmethod
    def from_dict(cls, data: dict) -> "ScheduleEntry":
        return cls(
            Id=data["Id"],
            ActiveDays=data["ActiveDays"],
            StartTime=time(hour=data["StartTimeHour"], minute=data["StartTimeMin"]),
            EndTime=time(hour=data["EndTimeHour"], minute=data["EndTimeMin"]),
        )
