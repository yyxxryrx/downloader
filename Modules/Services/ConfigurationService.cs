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
    
    public int ThreadCount
    {
        get => Get("ThreadCount", 8);
        set => Set("ThreadCount", value);
    }
}