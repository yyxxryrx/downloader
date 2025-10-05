using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Downloader.Modules.MainWindow.ViewModels;
using Downloader.Modules.Models;
using Downloader.Modules.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Downloader.Modules.MainWindow.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DownloadingPage : Page
{
    public DownloadingPageViewMode ViewModel => (DataContext as DownloadingPageViewMode)!;

    public DownloadingPage()
    {
        InitializeComponent();
        DataContext = new DownloadingPageViewMode();
    }
}

public class StatusConvertToIcon : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not DownloadStatus status) return value;
        return status switch
        {
            DownloadStatus.Pending => "",
            DownloadStatus.Downloading => "\uE896",
            DownloadStatus.Completed => "\uEC61",
            DownloadStatus.Failed => "\uEB90",
            DownloadStatus.Merging => "\uE7C4",
            DownloadStatus.Pause => "\uE769",
            DownloadStatus.Canceled => "\uE74D",
            _ => ""
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class StatueConvertToVisibilityForProgress : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not DownloadStatus status) return value;
        return status switch
        {
            DownloadStatus.Pending => Visibility.Visible,
            _ => Visibility.Collapsed,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class StatueConvertToVisibilityForIcon : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not DownloadStatus status) return value;
        return status switch
        {
            DownloadStatus.Pending => Visibility.Collapsed,
            _ => Visibility.Visible,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class StatueConvertToBoolForShowError : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not DownloadStatus status) return false;
        return status switch
        {
            DownloadStatus.Failed => true,
            _ => false,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class StatueConvertToBoolForIsIndeterminate : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not DownloadStatus status) return false;
        return status switch
        {
            DownloadStatus.Pending => true,
            DownloadStatus.Merging => true,
            _ => false
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class StatueConvertToBoolForShowPaused : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not DownloadStatus status) return false;
        return status switch
        {
            DownloadStatus.Pause => true,
            _ => false
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class ProgressConvertToString : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not double progress) return value;
        return progress >= 0 ? $"{progress:00.00} %" : "-- %";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class CountConvertToVisibilityForListView : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not int count) return value;
        return count > 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class CountConvertToVisibilityForTextblock : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not int count) return value;
        return count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class FilesizeToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not long size || size < 0) return "Unknown";
        var result = Utils.FilesizeConverters.iB.Convert(size);
        return $"{result.Value:0.00} {result.Unit}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class TimeToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not long time || time < 0) return "--:--";
        var mins = time / 60;
        var secs = time % 60;
        return $"{mins:00}:{secs:00}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}