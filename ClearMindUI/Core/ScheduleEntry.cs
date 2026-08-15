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
        public bool IsSelected { get; set; }

        [JsonIgnore]
        public string Days => ActiveDays.Count == 7 ? "Everyday"
                             : ActiveDays.Count == 0 ? "None"
                             : string.Join(", ", ActiveDays);

        [JsonIgnore]
        public string TimeRange => $"{StartTimeHour:D2}:{StartTimeMin:D2} - {EndTimeHour:D2}:{EndTimeMin:D2}";
    }
}
