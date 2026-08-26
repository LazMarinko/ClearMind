using System;
using System.IO;
using System.Text.Json;

namespace ClearMindUI
{
    // Class that handles everything related to the engine_status json file
    public static class EngineStatusStore
    {
        // Resolves to the current Windows account's AppData\ClearMind folder
        private static readonly string DirPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ClearMind");

        // Concrete path to the engine_status.json file
        public static readonly string FilePath = Path.Combine(DirPath, "engine_status.json");

        // Serializer to be used to read the engine_status.json file, with case-insensitive property names
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        // Static method used to read the contents of the engine_status.json file and return the deserialized EngineStatus object
        public static EngineStatus? Read()
        {
            if (!File.Exists(FilePath))
                return null;

            try
            {
                var json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<EngineStatus>(json, JsonOptions);
            }
            catch (IOException)
            {
                return null;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        // Static method used to create a FileSystemWatcher that monitors changes to the engine_status.json file
        public static FileSystemWatcher CreateWatcher(Action onChanged)
        {
            Directory.CreateDirectory(DirPath);

            var watcher = new FileSystemWatcher(DirPath, "engine_status.json") // Creates a new watcher and sets the path to the json
            {
                NotifyFilter = NotifyFilters.LastWrite, // Narrows the watcher to content writes only, ignoring renames/attribute changes
                EnableRaisingEvents = true, // Constructs it in an on state so that it starts watching immediately
            };

            // Sets the watcher to use the passed-in action when the engine_status.json file is created/changed
            // We discard the returned arguments since they are not needed in this use case
            watcher.Changed += (_, _) => onChanged();
            watcher.Created += (_, _) => onChanged();

            return watcher;
        }
    }
}
