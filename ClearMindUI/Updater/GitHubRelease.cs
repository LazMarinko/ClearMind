using System.Text.Json.Serialization;

namespace ClearMindUI.Updater
{
    // Class used to deserialize the JSON response from the GitHub API for the latest release
    internal class GitHubRelease
    {
        [JsonPropertyName("tag_name")]
        public required string TagName { get; init; }

        [JsonPropertyName("html_url")]
        public required string HtmlUrl { get; init; }

        [JsonPropertyName("assets")]
        public required GitHubAsset[] Assets { get; init; }
    }
}
