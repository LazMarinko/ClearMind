using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClearMindUI
{
    public class ScheduleEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<string> ActiveDays { get; set; } = new();
        public int StartTimeHour { get; set; } = 0;
        public int StartTimeMin { get; set; } = 0;
        public int EndTimeHour { get; set; } = 23;
        public int EndTimeMin { get; set; } = 59;

        [JsonIgnore]
        public bool IsSelected { get; set; } // Bool used to indicated to the UI whether the schedule is selected.

        [JsonIgnore]
        // String used to display the days of the week in the UI.
        public string Days => ActiveDays.Count == 7 ? "Everyday" // Displayed if every day is selected
                             : ActiveDays.Count == 0 ? "None" // Displayed if none are selected 
                             : string.Join(", ", ActiveDays); // Displays the selected days seperated by ", "

        [JsonIgnore]
        public string TimeRange => $"{StartTimeHour:D2}:{StartTimeMin:D2} - {EndTimeHour:D2}:{EndTimeMin:D2}";
    }
}
