using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net.Http;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Downloader.Modules.AskWindow.Models;

namespace Downloader.Modules.AskWindow.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty] Uri _url;
    [ObservableProperty] private bool _isCompleted;
    [ObservableProperty] private string _fileName = "";
    public CloseWindow? CloseWindow { get; set; }

    public async void LoadUri(Uri uri)
    {
        Url = uri;
        FileName = Path.GetFileName(uri.AbsolutePath);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        if (query["path"] is { } path)
            FileName = Path.GetFileName(path);
        using var client = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Head, uri);
        var response = await client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            if (response.Content.Headers.ContentDisposition?.FileName is { } fileName)
                FileName = fileName;
        }

        IsCompleted = true;
    }

    [RelayCommand]
    private void Close()
    {
        CloseWindow?.Invoke();
    }
}