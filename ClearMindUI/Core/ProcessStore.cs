using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ClearMindUI
{
    // Static class that handles thek saving, loading and deleting of locked process entries to and from a JSON file.
    public static class ProcessStore
    {
        // File path where the processes.json is stored
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ClearMind", "processes.json");

        // Static method for loading the list of locked process entries from the JSON file.
        public static List<LockedProcessEntry> Load()
        {
            if (!File.Exists(FilePath)) // Chekcs if the processes.json file exists, if not returns an empty list.
                return new List<LockedProcessEntry>();

            var json = File.ReadAllText(FilePath); // Stores all the text from the processes.json file into a string variable called json.
            return JsonSerializer.Deserialize<List<LockedProcessEntry>>(json) ?? new List<LockedProcessEntry>(); // Deserializes the json string into a list of LockedProcessEntry objects and returns it. If deserialization fails, returns an empty list.
        }

        // Static method for saving a locked process entry to the JSON file.
        public static void Save(LockedProcessEntry process)
        {
            var processes = Load(); // Retrieves the current list of locked process entries by calling the Load method.
            var index = processes.FindIndex(p => p.Id == process.Id); // Used to check if the process already exists in the list by finding its index based on its Id.

            if (index >= 0) // Check for the existance of the process that is being saved.
                processes[index] = process; // Updates the values if it already exists.
            else
                processes.Add(process); // Adds the new process to the list if it does not already exist.

            var directory = Path.GetDirectoryName(FilePath)!;
            Directory.CreateDirectory(directory);

            var json = JsonSerializer.Serialize(processes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        public static void Remove(LockedProcessEntry process)
        {
            var processes = Load();
            processes.RemoveAll(p => p.Id == process.Id);

            var directory = Path.GetDirectoryName(FilePath)!;
            Directory.CreateDirectory(directory);

            var json = JsonSerializer.Serialize(processes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
    }
}
