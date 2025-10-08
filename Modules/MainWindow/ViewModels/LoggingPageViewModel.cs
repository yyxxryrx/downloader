using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Downloader.Modules.Models;
using Microsoft.UI.Xaml.Controls;
using Serilog.Events;

namespace Downloader.Modules.MainWindow.ViewModels;

public class LoggingPageViewModel : INotifyPropertyChanged, IDisposable
{
    public List<LogEntry> Logs => GlobalVars.UiSink.Logs.Where(log => SelectItems.Contains(log.Level)).ToList();

    public List<LogEventLevel> ItemsSource { get; } =
    [
        LogEventLevel.Debug,
        LogEventLevel.Information,
        LogEventLevel.Warning,
        LogEventLevel.Error,
        LogEventLevel.Fatal
    ];

    public List<LogEventLevel> InitSelectItems { get; } = 
    [
        LogEventLevel.Information,
        LogEventLevel.Warning,
        LogEventLevel.Error,
        LogEventLevel.Fatal
    ]; 

    private List<LogEventLevel> SelectItems { get; } = [];

    public LoggingPageViewModel()
    {
        GlobalVars.UiSink.Logs.CollectionChanged += LogsOnCollectionChanged;
    }

    private void LogsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Logs));
    }

    public void Selector_OnSelectionChanged(SelectionChangedEventArgs e)
    {
        foreach (var logEventLevel in e.RemovedItems.OfType<LogEventLevel>())
            SelectItems.Remove(logEventLevel);
        SelectItems.AddRange(e.AddedItems.OfType<LogEventLevel>());
        OnPropertyChanged(nameof(Logs));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Dispose()
    {
        GlobalVars.UiSink.Logs.CollectionChanged -= LogsOnCollectionChanged;
    }
}