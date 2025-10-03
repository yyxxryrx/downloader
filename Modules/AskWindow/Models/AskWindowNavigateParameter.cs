using System;

namespace Downloader.Modules.AskWindow.Models;

public delegate void CloseWindow();

public class AskWindowNavigateParameter(string url, CloseWindow closeWindow)
{
    public string Url { get; init; } = url;
    public CloseWindow CloseWindow { get; init; } = closeWindow;
}