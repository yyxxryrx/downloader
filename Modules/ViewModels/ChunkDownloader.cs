using CommunityToolkit.Mvvm.ComponentModel;
using Downloader.Modules.Models;

namespace Downloader.Modules.ViewModels;

public partial class ChunkDownloader : ObservableObject
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string Url { get; set; }
    public string Path { get; set; }
    [ObservableProperty] private double _progress = -1;
    [ObservableProperty] private DownloadStatue _statue;
}