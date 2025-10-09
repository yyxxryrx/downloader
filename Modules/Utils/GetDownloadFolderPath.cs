using System;
using Downloader.Modules.Services;
using Serilog;

namespace Downloader.Modules.Utils;

public static class GetDownloadFolderPath
{
    public static string? GetPath()
    {
        if (
            DllImportService.SHGetKnownFolderPath(
                DllImportService.DownloadsFolderGuid,
                0,
                IntPtr.Zero,
                out var path
            ) == 0
        )
            return path;
        Log.Warning("Cannot get download folder path");
        return null;
    }
}