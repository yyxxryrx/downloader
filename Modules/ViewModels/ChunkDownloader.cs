using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Downloader.Modules.Models;
using Microsoft.UI.Dispatching;

namespace Downloader.Modules.ViewModels;

public record DownloadRange(long Start, long End)
{
    public DownloadRange UpdateStart(long value) => this with { Start = Start + value };
}

public delegate void ChunkDownloadEvent(ChunkDownloader sender);

public delegate void ChunkDownloadEventWithMessage(ChunkDownloader sender, string message);

public partial class ChunkDownloader(HttpClient client) : ObservableObject
{
    public required int Id { get; init; }
    public string FilePath => Path.Join(TargetPath, FileName + $".part{Id}");
    private HttpClient Client { get; } = client;
    public required string FileName { get; init; }
    public required Uri Url { get; init; }
    public required string TargetPath { get; init; }
    [ObservableProperty] private double _progress = -1;
    [ObservableProperty] private long _speed = -1;
    [ObservableProperty] private DownloadStatus _status;

    public required long TotalSize { get; init; }
    [ObservableProperty] private long _downloadTotal;

    public required DownloadRange Range;
    private CancellationTokenSource _tokenSource = new();
    private long __downloadTotal;

    public ChunkDownloadEvent? OnDownloadCompleted { get; set; }
    public ChunkDownloadEventWithMessage? OnDownloadFailed { get; set; }
    public ChunkDownloadEvent? OnDownloadPause { get; set; }

    private long _lastTime;
    private long _lastDownloadTotal;

    private async Task<string?> Download(int retryTimes, CancellationToken token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, Url);
        Debug.WriteLine($"Id: {Id}, Range: {Range}");
        var range = Range.UpdateStart(__downloadTotal);
        request.Headers.Range = new RangeHeaderValue(range.Start, range.End - 1);

        var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);
        await using var contentStream = await response.Content.ReadAsStreamAsync(token);
        const int bufferSize = 8192;
        var buffer = new byte[bufferSize];
        try
        {
            await using var fileStream = new FileStream(
                FilePath,
                FileMode.Append,
                FileAccess.Write,
                FileShare.None,
                bufferSize,
                true
            );
            int bytesRead;
            while ((bytesRead = await contentStream.ReadAsync(buffer.AsMemory(0, bufferSize), token)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), token);
                __downloadTotal += bytesRead;
                // Debug.WriteLine($"Downloaded: {__downloadTotal}, $Id: {Id}");
            }
        }
        catch (TaskCanceledException)
        {
            return null;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            Debug.WriteLine($"{Id}: Retry {retryTimes} times");
            if (retryTimes < 3)
                return await Download(++retryTimes, token);
            return e.Message;
        }

        return null;
    }

    public async Task Start()
    {
        if (Status is DownloadStatus.Downloading)
            return;
        _lastTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        Speed = 0;
        Status = DownloadStatus.Downloading;
        if (_tokenSource.IsCancellationRequested)
            _tokenSource = new CancellationTokenSource();
        var errMessage = await Download(0, _tokenSource.Token);
        if (errMessage != null)
        {
            Status = DownloadStatus.Failed;
            OnDownloadFailed?.Invoke(this, errMessage);
            return;
        }

        if (Status == DownloadStatus.Pause)
        {
            Speed = -1;
            OnDownloadPause?.Invoke(this);
            return;
        }

        OnDownloadCompleted?.Invoke(this);
    }

    public long UpdateInfo()
    {
        var total = __downloadTotal;
        DownloadTotal = total;
        var added = total - _lastDownloadTotal;
        _lastDownloadTotal = total;
        
        if (added == 0) 
            return total;
            
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var offset = now - _lastTime;
        _lastTime = now;

        if (offset <= 0) return total;
        Speed = added / offset * 1000;
        Progress = total / (double)TotalSize * 100;

        return total;
    }

    public void Pause()
    {
        if (Status is DownloadStatus.Pause)
            return;
        Status = DownloadStatus.Pause;
        _tokenSource.Cancel();
    }
}