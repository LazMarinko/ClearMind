using System;

namespace ClearMindUI.Updater
{
    // Class thats used as the return Value of the CheckForUpdateAsync method
    public class UpdateInfo
    {
        public required Version Version { get; init; }
        public required string DownloadUrl { get; init; }
    }
}
