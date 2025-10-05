namespace Downloader.Modules.AskWindow.Models;

public delegate void CloseWindow();

public class AskWindowNavigateParameter(string url, CloseWindow closeWindow)
{
    public string Url { get; } = url;
    public CloseWindow CloseWindow { get; } = closeWindow;
}