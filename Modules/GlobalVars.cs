using System.Collections.ObjectModel;
using Downloader.Modules.Services;

namespace Downloader.Modules;

public static class GlobalVars
{
    public static ObservableCollection<Downloader.Modules.ViewModels.Downloader> Downloaders = [];
    public static ConfigurationService ConfigurationService = new();
}