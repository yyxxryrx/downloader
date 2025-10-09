using System;

namespace Downloader.Modules.Models;

public class HistoryEntry
{
    public long Id { get; set; }
    public string Filename { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string Browser { get; init; } = string.Empty;
    public DateTime DateTime { get; init; }
    public long FileSize { get; init; }
    public DownloadStatus Status { get; init; }
}