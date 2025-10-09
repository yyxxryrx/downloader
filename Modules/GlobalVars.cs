using System.Collections.ObjectModel;
using Downloader.Modules.Services;
using Downloader.Modules.ViewModels;

namespace Downloader.Modules;

public static class GlobalVars
{
    public static ObservableCollection<Downloader.Modules.ViewModels.Downloader> Downloaders = [];
    public static readonly ConfigurationService ConfigurationService = new();
    public static readonly UiSink UiSink = new();
}