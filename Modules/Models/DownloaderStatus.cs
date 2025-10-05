namespace Downloader.Modules.Models;

public enum DownloadStatus
{
    Pending,
    Downloading,
    Pause,
    Merging,
    Completed,
    Failed,
    Canceled
}