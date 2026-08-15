using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClearMindUI
{

    // Class which objects are stored in the JSON file. Represents a locked process with its name and associated schedules.
    public class LockedProcessEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Unique identifier for the locked process entry.
        public string Name { get; set; } = string.Empty;
        public List<ScheduleEntry> Schedules { get; set; } = new() { new ScheduleEntry() };

        [JsonIgnore]
        public bool IsExpanded { get; set; } = true; // Indicates whether the entry is expanded in the UI to show its schedules.

        [JsonIgnore]
        public bool IsComplex => Schedules.Count > 1; // Indicates whether the entry has multiple schedules.

        [JsonIgnore]
        public bool ShowSchedules => IsComplex && IsExpanded; // Indicates whether to show the schedules in the UI.

        [JsonIgnore]
        public string SummaryText => IsComplex // Used to display information about the schedules in the UI.
            ? $"{Schedules.Count} schedules" // Shows the amount of schedules if there are multiple.
            : $"{Schedules[0].Days}  •  {Schedules[0].TimeRange}"; // Shows the days and time range of the single schedule if there is only one.
    }
}
