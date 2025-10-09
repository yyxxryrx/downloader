using Windows.Storage;

namespace Downloader.Modules.Services;

public class ConfigurationService
{
    private ApplicationDataContainer Configuration { get; } = ApplicationData.Current.LocalSettings;
    private T Get<T>(string key, T defaultValue) => Configuration.Values[key] is T t ? t : defaultValue;
    private void Set<T>(string key, T value) => Configuration.Values[key] = value;

    public string Language
    {
        get => Get("Language", string.Empty);
        set => Set("Language", value);
    }
    
    public string DownloadPath
    {
        get => Get("DownloadPath", string.Empty);
        set => Set("DownloadPath", value);
    }
    
    public string DownloadPathMode
    {
        get => Get("DownloadPathMode", "Default");
        set => Set("DownloadPathMode", value);
    }
    
    public int ThreadCount
    {
        get => Get("ThreadCount", 8);
        set => Set("ThreadCount", value);
    }
}