using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Downloader.Modules.Models;
using Microsoft.UI.Dispatching;

namespace Downloader.Modules.Services;

public class HistoryManagerService
{
    private static readonly SqliteService SqliteService = new();
    public ObservableCollection<HistoryEntry> HistoryEntries = [];
    private bool _isInitialized;
    private DispatcherQueue DispatcherQueue { get; } = DispatcherQueue.GetForCurrentThread();

    public async Task Initialize()
    {
        if (_isInitialized) return;
        foreach (var historyEntry in await SqliteService.GetAll())
            HistoryEntries.Add(historyEntry);
        _isInitialized = true;
    }

    public async Task Add(HistoryEntry historyEntry)
    {
        var id = await SqliteService.Add(
            historyEntry.Filename,
            historyEntry.Url,
            historyEntry.Browser,
            historyEntry.FileSize,
            historyEntry.DateTime
        );
        if (id < 0) return;
        historyEntry.Id = id;
        if (DispatcherQueue.HasThreadAccess)
            HistoryEntries.Insert(0, historyEntry);
        else
            DispatcherQueue.TryEnqueue(() => HistoryEntries.Insert(0, historyEntry));
    }
}