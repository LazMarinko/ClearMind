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

        public static List<LockedProcessEntry> Load()
        {
            if (!File.Exists(FilePath))
                return new List<LockedProcessEntry>();

            var json = File.ReadAllText(FilePath);
            // Deserializes the json string into a list of LockedProcessEntry objects and returns it.
            // If deserialization fails, returns an empty list.
            return JsonSerializer.Deserialize<List<LockedProcessEntry>>(json) ?? new List<LockedProcessEntry>(); 
        }

        public static void Save(LockedProcessEntry process)
        {
            var processes = Load();
            var index = processes.FindIndex(p => p.Id == process.Id);

            // If the process already exists, update it; otherwise, add it to the list.
            if (index >= 0)
                processes[index] = process;
            else
                processes.Add(process);

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
