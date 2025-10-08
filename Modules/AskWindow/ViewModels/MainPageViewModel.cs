using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Downloader.Modules.AskWindow.Models;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Animation;
using Serilog;

namespace Downloader.Modules.AskWindow.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CanDownload))]
    Uri? _url;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CanDownload))]
    private bool _isCompleted = true;

    [ObservableProperty] private string _fileName = "";
    [ObservableProperty] private long _fileSize = -1;

    public bool CanDownload => Url is not null && IsCompleted;

    public Storyboard? StartStoryboard { get; init; }
    public Storyboard? EndStoryboard { get; init; }
    public DispatcherQueue? DispatcherQueue { get; init; }
    public CloseWindow? CloseWindow { get; set; }

    private bool AllowRange { get; set; }

    public async void LoadUri(Uri? uri)
    {
        if (uri is null) return;
        IsCompleted = false;
        FileName = Path.GetFileName(uri.AbsolutePath);
        AllowRange = false;
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        if (query["path"] is { } path)
            FileName = Path.GetFileName(path);
        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);
        using var request = new HttpRequestMessage(HttpMethod.Head, uri);
        request.Headers.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:143.0) Gecko/20100101 Firefox/143.0");
        try
        {
            var response = await client.SendAsync(request);
            Debug.WriteLine(response);
            Log.Debug("File info response: {@Response}", response);
            if (response.IsSuccessStatusCode)
            {
                if (response.Content.Headers.ContentDisposition?.FileName is { } fileName)
                    FileName = fileName;
                FileSize = response.Content.Headers.ContentLength ?? -1;
                AllowRange = response.Headers.TryGetValues("Accept-Ranges", out var values) &&
                             values.Any(i => string.Equals(i, "bytes", StringComparison.OrdinalIgnoreCase));
            }
            Log.Information("File info, name: {Filename}, url: {Url}", FileName, uri);
        }
        catch (Exception ex)
        {
            if (ex is not TaskCanceledException)
                Log.Error(ex, "get file info error");
            FileSize = -1;
        }

        Url = uri;

        IsCompleted = true;
    }

    [RelayCommand]
    private void Close()
    {
        CloseWindow?.Invoke();
    }

    [RelayCommand]
    private void CopyUrl()
    {
        if (Url is null) return;
        var package = new DataPackage();
        package.SetText(Url.AbsoluteUri);
        Clipboard.SetContent(package);
        if (StartStoryboard?.GetCurrentState() != ClockState.Stopped ||
            EndStoryboard?.GetCurrentState() != ClockState.Stopped)
            return;
        DispatcherQueue?.TryEnqueue(async void () =>
        {
            try
            {
                StartStoryboard.Begin();
                await Task.Delay(1500);
                EndStoryboard.Begin();
                await Task.Delay(300);
                EndStoryboard.Stop();
                StartStoryboard.Stop();
            }
            catch (Exception e)
            {
                Log.Error(e, "Play storyboard error");
            }
        });
    }

    [RelayCommand]
    private void Retry()
    {
        LoadUri(Url);
    }

    [RelayCommand]
    private void NewDownloader()
    {
        if (Url is null) return;
        Log.Information("Download {Filename} start, url: {Url}", FileName, Url);
        var downloader = new Modules.ViewModels.Downloader
        {
            FileName = FileName,
            TargetPath = "D:/Downloads/abcd",
            Url = Url,
            TotalSize = FileSize,
            AllowRange = AllowRange,
            ThreadCount = GlobalVars.ConfigurationService.ThreadCount
        };
        Log.Debug("Downloader: {@Downloader}", downloader);
        downloader.Init();
        GlobalVars.Downloaders.Add(downloader);
        CloseWindow?.Invoke();
    }
}