using System;
using System.Collections.ObjectModel;
using Downloader.Modules.Services;
using Downloader.Modules.ViewModels;
using Serilog.Events;

namespace Downloader.Modules;

public static class GlobalVars
{
    public static ObservableCollection<Downloader.Modules.ViewModels.Downloader> Downloaders = [];
    public static ConfigurationService ConfigurationService = new();
    public static UiSink UiSink = new();
}