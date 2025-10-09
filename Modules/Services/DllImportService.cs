using System;
using System.Runtime.InteropServices;

namespace Downloader.Modules.Services;

public static class DllImportService
{
    public static readonly Guid DownloadsFolderGuid = new("374DE290-123F-4565-9164-39C4925E467B");

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern int SHGetKnownFolderPath(
        [MarshalAs(UnmanagedType.LPStruct)] Guid rfid,
        uint dwFlags,
        IntPtr hToken,
        out string pszPath
    );
}