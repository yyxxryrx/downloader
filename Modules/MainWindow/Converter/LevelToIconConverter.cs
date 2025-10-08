using System;
using Microsoft.UI.Xaml.Data;
using Serilog.Events;

namespace Downloader.Modules.MainWindow.Converter;
public class LevelToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not LogEventLevel level) return "\uF167";
        return level switch
        {
            LogEventLevel.Debug => "\uEBE8",
            LogEventLevel.Information => "\uF167",
            LogEventLevel.Warning => "\uE814",
            LogEventLevel.Error => "\uEB90",
            LogEventLevel.Fatal => "\uE711",
            _ => "\uF167"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}