using Windows.Storage;

namespace Downloader.Modules.Services;

public class ConfigurationService
{
    private ApplicationDataContainer Configuration { get; } = ApplicationData.Current.LocalSettings;
    private T Get<T>(string key, T defaultValue) where T : class => Configuration.Values[key] as T ?? defaultValue;
    private void Set<T>(string key, T value) where T : class => Configuration.Values[key] = value;

    public string Language
    {
        get => Get("Language", string.Empty);
        set => Set("Language", value);
    }
}