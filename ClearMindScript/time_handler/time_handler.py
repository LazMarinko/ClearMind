import http.client
from datetime import time, date, datetime, timedelta
from email.utils import parsedate_to_datetime

from json_to_obj_classes.lockedProcessEntry import LockedProcessEntry

_WEEKDAY_CODES = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"]

MIDNIGHT = time.min


# Used to get today's weekday code, e.g. "Mon", matching the ActiveDays format
def today_code() -> str:
    return _WEEKDAY_CODES[date.today().weekday()]


# Used to find either the latest end time or the earliest start time across all processes for the current day
def find_boundary_time(locked_processes: list[LockedProcessEntry], latest: bool) -> time | None:
    today = today_code()

    # List of the relevant times for the current day, end times if latest else start times
    times = [
        (schedule.EndTime if latest else schedule.StartTime)
        for process in locked_processes
        for schedule in process.Schedules
        if today in schedule.ActiveDays
    ]

    if not times:
        return None

    if latest:
        return max(times)
    else:
        return min(times)


# Used to get the current time from the internet, falling back to the local clock if offline
def get_current_time() -> time:
    try:
        connection = http.client.HTTPSConnection("www.google.com", timeout=3)
        connection.request("HEAD", "/")
        response = connection.getresponse()
        date_header = response.getheader("Date")
        connection.close()

        # Date header comes back in GMT, convert it to the local timezone before use
        return parsedate_to_datetime(date_header).astimezone().time()
    except (OSError, http.client.HTTPException, TypeError, ValueError):
        return datetime.now().time() # Fallback in case the user has no internet connection returns system time.


# Used to work out how long to sleep for until a given time, rolling over to tomorrow if that time already passed today
def seconds_until_time(target_time: time) -> float:
    now = datetime.now()
    target = datetime.combine(date.today(), target_time)

    if target <= now:
        target += timedelta(days=1)

    return (target - now).total_seconds()
