using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;

namespace ClearMindUI.Updater
{
    // Class used to check for updates of the ClearMind application
    public static class VersionChecker
    {
        private const string LatestReleaseUrl = "https://api.github.com/repos/LazMarinko/ClearMind/releases/latest";

        // HttpClient used to send requests to GitHub
        private static readonly HttpClient client = new();

        // Static constructer used to ensure that there is only 1 client for each instance of the class
        static VersionChecker()
        {
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("ClearMindUI", GetCurrentVersion().ToString()));
        }

        public static Version GetCurrentVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version!;
        }

        // Asynchronous method used to check for updates
        public static async Task<UpdateInfo?> CheckForUpdateAsync()
        {
            try
            {
                var release = await client.GetFromJsonAsync<GitHubRelease>(LatestReleaseUrl); // fetches the current version from github

                // Checks if the release is null or if the version is not valid, if they are not it sets puts the latestVersion value into latestVersion variable
                if (release is null || !Version.TryParse(release.TagName.TrimStart('v'), out var latestVersion))
                    return null;

                if (latestVersion <= GetCurrentVersion())
                    return null;

                // Fetches the download url from the assets page, if that fails it attempts to fetch the browser download url, if that fails it will just give the html url
                var downloadUrl = Array.Find(release.Assets, a => a.Name == "ClearMindSetup.exe")?.BrowserDownloadUrl
                                   ?? release.HtmlUrl; 

                return new UpdateInfo { Version = latestVersion, DownloadUrl = downloadUrl };
            }
            catch (Exception ex) when (ex is HttpRequestException or NotSupportedException or System.Text.Json.JsonException or TaskCanceledException)
            {
                return null;
            }
        }
    }
}
