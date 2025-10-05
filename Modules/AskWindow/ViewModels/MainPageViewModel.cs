using System;
using System.Collections;
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

namespace Downloader.Modules.AskWindow.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty] Uri _url;
    [ObservableProperty] private bool _isCompleted = true;
    [ObservableProperty] private string _fileName = "";
    [ObservableProperty] private long _fileSize = -1;

    public Storyboard? StartStoryboard { get; init; }
    public Storyboard? EndStoryboard { get; init; }
    public DispatcherQueue? DispatcherQueue { get; init; }
    public CloseWindow? CloseWindow { get; set; }

    public async void LoadUri(Uri uri)
    {
        Url = uri;
        IsCompleted = false;
        FileName = Path.GetFileName(uri.AbsolutePath);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        if (query["path"] is { } path)
            FileName = Path.GetFileName(path);
        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);
        using var request = new HttpRequestMessage(HttpMethod.Head, uri);
        request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:143.0) Gecko/20100101 Firefox/143.0");
        var response = await client.SendAsync(request);
        Debug.WriteLine(response);
        if (response.IsSuccessStatusCode)
        {
            if (response.Content.Headers.ContentDisposition?.FileName is { } fileName)
                FileName = fileName;
            FileSize = response.Content.Headers.ContentLength ?? -1;
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
        var package = new DataPackage();
        package.SetText(Url.AbsoluteUri);
        Clipboard.SetContent(package);
        if (StartStoryboard?.GetCurrentState() != ClockState.Stopped ||
            EndStoryboard?.GetCurrentState() != ClockState.Stopped)
            return;
        DispatcherQueue?.TryEnqueue(async void () =>
        {
            StartStoryboard.Begin();
            await Task.Delay(1500);
            EndStoryboard.Begin();
            await Task.Delay(300);
            EndStoryboard.Stop();
            StartStoryboard.Stop();
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
        var downloader = new Modules.ViewModels.Downloader
        {
            FileName = FileName,
            TargetPath = "D:/Downloads/abcd",
            Url = Url,
            TotalSize = FileSize,
            AllowRange = true,
            ThreadCount = 24
        };
        downloader.Init();
        GlobalVars.Downloaders.Add(downloader);
        CloseWindow?.Invoke();
    }
}