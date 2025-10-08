using System.Collections.ObjectModel;
using Downloader.Modules.Models;
using Microsoft.UI.Dispatching;
using Serilog.Core;
using Serilog.Events;

namespace Downloader.Modules.ViewModels;

public class UiSink : ILogEventSink
{
    public ObservableCollection<LogEntry> Logs { get; } = [];
    private DispatcherQueue DispatcherQueue { get; } = DispatcherQueue.GetForCurrentThread();

    public void Emit(LogEvent logEvent)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            Logs.Insert(0, new LogEntry
            {
                Level = logEvent.Level,
                Message = logEvent.RenderMessage(),
                Timestamp = logEvent.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
            });
            if (Logs.Count > 1000)
                Logs.RemoveAt(1000);
        });
    }
}