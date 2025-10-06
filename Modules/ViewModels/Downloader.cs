using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Timers;
using CommunityToolkit.Mvvm.Input;
using Downloader.Modules.Models;
using Microsoft.UI.Dispatching;

namespace Downloader.Modules.ViewModels;

record RangeItem(long Start, long End, long Size);

public partial class Downloader
    : ObservableObject
{
    [ObservableProperty] private long _speed;
    [ObservableProperty] private double _progress = -1;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsPaused))] [NotifyPropertyChangedFor(nameof(IsDownloading))]
    private DownloadStatus _status;

    [ObservableProperty] private long _downloadTotal;
    [ObservableProperty] private long _remainingTime = -1;
    public ObservableCollection<ChunkDownloader> RunningChunks = [];
    private List<ChunkDownloader> CompletedChunks { get; } = [];
    public required string TargetPath { get; init; }
    private string FilePath => Path.Join(TargetPath, FileName);
    public int ThreadCount { get; init; } = 8;

    private long _lastTime;

    public required Uri Url { get; init; }
    public long? TotalSize { get; init; }
    public required bool AllowRange { get; init; }
    public required string FileName { get; init; }

    private Timer Timer { get; } = new(TimeSpan.FromMilliseconds(150))
    {
        AutoReset = true,
    };

    private DispatcherQueue DispatcherQueue { get; } = DispatcherQueue.GetForCurrentThread();

    public bool IsPaused => Status is DownloadStatus.Pause;
    public bool IsDownloading => Status is DownloadStatus.Downloading;

    public void Init()
    {
        var handler = new HttpClientHandler
        {
            AllowAutoRedirect = true,
        };
        var client = new HttpClient(handler);
        client.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:143.0) Gecko/20100101 Firefox/143.0");
        client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
        {
            NoCache = true,
            NoStore = true,
            MaxAge = TimeSpan.Zero,
            NoTransform = true,
        };
        client.DefaultRequestHeaders.Pragma.ParseAdd("no-cache");
        Timer.Elapsed += (_, _) => UpdateInfo();
        if (AllowRange)
        {
            var chunkSize = TotalSize!.Value / ThreadCount;
            var ranges = Enumerable.Range(0, ThreadCount)
                .Select(i => new RangeItem(i * chunkSize, (i + 1) * chunkSize, chunkSize))
                .ToList();
            ranges[^1] = new RangeItem(ranges[^1].Start, TotalSize!.Value + 1, TotalSize!.Value - ranges[^1].Start);
            var tasks = ranges.Select((range, id) => new ChunkDownloader(client)
                {
                    Id = id,
                    Range = new DownloadRange(range.Start, range.End),
                    FileName = FileName,
                    TargetPath = TargetPath,
                    TotalSize = range.Size,
                    Url = Url,
                })
                .ToList();
            RunningChunks = new ObservableCollection<ChunkDownloader>(tasks);
            foreach (var task in tasks)
            {
                task.OnDownloadCompleted = OnChunkDownloadCompleted;
                task.OnDownloadFailed = OnChunkDownloadFailed;
            }

            Task.Run(async void () =>
            {
                await RemoveTempFile();
                Resume();
            });
        }
    }

    private void UpdateInfo()
    {
        Debug.WriteLine("UpdateInfo called");
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var offset = now - _lastTime;
        if (offset == 0)
            return;

        DispatcherQueue.TryEnqueue(() =>
        {
            var total = DownloadTotal;
            DownloadTotal = CompletedChunks.Select(i => i.TotalSize).Sum() +
                            RunningChunks.Select(i => i.UpdateInfo()).Sum();
            var added = DownloadTotal - total;
            if (added > 0 && offset > 0)
                Speed = added / offset * 1000;
            if (TotalSize is not { } totalSize) return;
            Progress = DownloadTotal / (double)totalSize * 100;
            if (Speed > 0)
                RemainingTime = (totalSize - DownloadTotal) / Speed;
            else
                RemainingTime = -1;
        });
        _lastTime = now;
    }

    [RelayCommand]
    private void Pause()
    {
        if (Status is DownloadStatus.Pause) return;
        Timer.Stop();
        foreach (var chunk in RunningChunks)
            chunk.Pause();
        if (DispatcherQueue.HasThreadAccess)
            Status = DownloadStatus.Pause;
        else
            DispatcherQueue.TryEnqueue(() => Status = DownloadStatus.Pause);
        UpdateInfo();
    }

    [RelayCommand]
    private void Resume()
    {
        if (Status is DownloadStatus.Downloading) return;
        Timer.Start();
        foreach (var chunk in RunningChunks)
            _ = chunk.Start();
        if (DispatcherQueue.HasThreadAccess)
            Status = DownloadStatus.Downloading;
        else
            DispatcherQueue.TryEnqueue(() => Status = DownloadStatus.Downloading);
    }

    [RelayCommand]
    private void Delete()
    {
        if (Status is not DownloadStatus.Pause) return;
        foreach (var chunk in RunningChunks)
            chunk.Pause();
        RemoveTempFile();
        if (DispatcherQueue.HasThreadAccess)
            Status = DownloadStatus.Canceled;
        else
            DispatcherQueue.TryEnqueue(() => Status = DownloadStatus.Canceled);
        GlobalVars.Downloaders.Remove(this);
    }

    private Task RemoveTempFile()
    {
        // Clean the temp file
        foreach (var chunk in RunningChunks)
            File.Delete(chunk.FilePath);
        foreach (var chunk in CompletedChunks)
            File.Delete(chunk.FilePath);
        return Task.CompletedTask;
    }

    private async Task MergeChunks()
    {
        Status = DownloadStatus.Merging;
        CompletedChunks.Sort((a, b) => a.Id - b.Id);
        await Task.Run(() =>
        {
            using var file = File.Open(FilePath, FileMode.Create, FileAccess.Write);
            foreach (var chunk in CompletedChunks)
            {
                using var chunkFile = File.Open(chunk.FilePath, FileMode.Open, FileAccess.Read);
                chunkFile.CopyTo(file);
            }
        });
    }

    private void OnChunkDownloadFailed(ChunkDownloader sender, string message)
    {
        Status = DownloadStatus.Failed;
        Delete();
    }

    private async void OnChunkDownloadCompleted(ChunkDownloader sender)
    {
        UpdateInfo();
        await Task.Delay(TimeSpan.FromSeconds(1));
        DispatcherQueue.TryEnqueue(async () => {
            RunningChunks.Remove(sender);
            CompletedChunks.Add(sender);
            if (RunningChunks.Count != 0) return;
            Timer.Stop();
            Timer.Close();
            Timer.Dispose();
            await MergeChunks();
            await RemoveTempFile();
            Progress = 100;
            Status = DownloadStatus.Completed;
            await Task.Delay(TimeSpan.FromSeconds(3));
            GlobalVars.Downloaders.Remove(this);
        });
    }
}