using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.Windows.ApplicationModel.Resources;
using Serilog.Events;

namespace Downloader.Modules.MainWindow.Converter;

public class LevelToStringConverter : IValueConverter
{
    private ResourceLoader Loader { get; } = new();
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not LogEventLevel level) return "";
        try
        {
            return Loader.GetString(level.ToString());
        }
        catch
        {
            return level.ToString();
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}