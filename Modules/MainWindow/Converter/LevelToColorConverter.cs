using System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Serilog.Events;

namespace Downloader.Modules.MainWindow.Converter;

public class LevelToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not LogEventLevel level) return new SolidColorBrush();
        return (Application.Current.Resources[level switch
        {
            LogEventLevel.Debug => "SystemFillColorNeutralBrush",
            LogEventLevel.Information => "SystemFillColorAttentionBrush",
            LogEventLevel.Warning => "SystemFillColorCautionBrush",
            LogEventLevel.Error => "SystemFillColorCriticalBrush",
            LogEventLevel.Fatal => "SystemFillColorCriticalBrush",
            _ => "SystemFillColorSolidAttentionBackgroundBrush"
        }] as SolidColorBrush)!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
