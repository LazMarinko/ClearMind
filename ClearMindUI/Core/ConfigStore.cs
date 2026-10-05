using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;
namespace ClearMindUI.Core
{
    // Class that controls the reading and writing of the app_config.json file, which is used to store user preferences and settings for the ClearMind application
    class ConfigStore
    {
        private static readonly string configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ClearMind", "app_config.json");
        
        // Used to create the config file if it doesn't exist
        public static void CreateConfig()
        {
            if (File.Exists(configFilePath))
                return;

            Save(new Config());
        }

        // Method used to save edits made to the config to the app_config.json
        public static void Save(Config config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            var directory = Path.GetDirectoryName(configFilePath)!;

            Directory.CreateDirectory(directory);
            File.WriteAllText(configFilePath, json);
        }

        // Method used to read from the app_config.json. Returns a Config class
        public static Config Load()
        {
           
            var json = File.ReadAllText(configFilePath);

            return JsonSerializer.Deserialize<Config>(json);
        }
    }
}
