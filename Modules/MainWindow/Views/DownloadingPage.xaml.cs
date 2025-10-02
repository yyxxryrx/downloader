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
        ViewModel.Downloaders.Add(new Modules.ViewModels.Downloader("", "Test")
        {
            Chunks =
            [
                new ChunkDownloader
                {
                    Id = 0,
                    Statue = DownloadStatue.Downloading,
                    Progress = 50
                },
                new ChunkDownloader
                {
                    Id = 1,
                    Statue = DownloadStatue.Downloading,
                    Progress = 30
                },
                new ChunkDownloader
                {
                    Id = 2,
                    Statue = DownloadStatue.Downloading,
                    Progress = 40
                },
            ],
            Statue = DownloadStatue.Downloading,
            Progress = 20,
            Speed = 100,
        });
        ViewModel.Downloaders.Add(new Modules.ViewModels.Downloader("", "Test2")
        {
            Chunks =
            [
                new ChunkDownloader(),
                new ChunkDownloader(),
                new ChunkDownloader(),
            ],
            Statue = DownloadStatue.Pending,
            Progress = 0,
            Speed = 0,
        });
    }
}

public class StatueConvertToIcon : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not DownloadStatue status) return value;
        return status switch
        {
            DownloadStatue.Pending => "",
            DownloadStatue.Downloading => "\uE896",
            DownloadStatue.Completed => "\uEC61",
            DownloadStatue.Failed => "\uEB90",
            DownloadStatue.Merging => "\uE7C4",
            _ => value
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
        if (value is not DownloadStatue status) return value;
        return status switch
        {
            DownloadStatue.Pending => Visibility.Visible,
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
        if (value is not DownloadStatue status) return value;
        return status switch
        {
            DownloadStatue.Pending => Visibility.Collapsed,
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
        if (value is not DownloadStatue status) return false;
        return status switch
        {
            DownloadStatue.Failed => true,
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
        if (value is not DownloadStatue status) return false;
        return status switch
        {
            DownloadStatue.Pending => true,
            DownloadStatue.Merging => true,
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
        if (value is not DownloadStatue status) return false;
        return status switch
        {
            DownloadStatue.Pause => true,
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