using System.Text.Json.Serialization;

namespace ClearMindUI.Updater
{
    // Class used to store information about the assets of the latest release from the GitHub API
    internal class GitHubAsset
    {
        [JsonPropertyName("name")]
        public required string Name { get; init; }

        [JsonPropertyName("browser_download_url")]
        public required string BrowserDownloadUrl { get; init; }
    }
}
