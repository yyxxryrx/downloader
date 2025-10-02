using System.Collections.ObjectModel;

namespace Downloader.Modules;

public static class GlobalVars
{
    public static ObservableCollection<Downloader.Modules.ViewModels.Downloader> Downloaders = [];
}