using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using Downloader.Modules.AskWindow.Models;
using Downloader.Modules.AskWindow.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Downloader.Modules.AskWindow.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPageViewModel ViewModel => (DataContext as MainPageViewModel)!;

    public MainPage()
    {
        InitializeComponent();
        DataContext = new MainPageViewModel
        {
            DispatcherQueue = DispatcherQueue,
            EndStoryboard = CopyUrlButtonAnimationEnd,
            StartStoryboard = CopyUrlButtonAnimationStart,
        };
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is not AskWindowNavigateParameter parameter) return;
        ViewModel.CloseWindow = parameter.CloseWindow;
        try
        {
            if (!string.IsNullOrEmpty(parameter.Url))
                ViewModel.LoadUri(new Uri(parameter.Url));
        }
        catch (Exception ex)
        {
            var dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Error",
                Content = ex.Message,
                CloseButtonText = "OK"
            };
            dialog.ShowAsync();
        }
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

public class IsCompletedToVisibilityForGrid : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not bool isCompleted) return Visibility.Collapsed;
        return isCompleted ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class IsCompletedToVisibilityForProgressRing : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not bool isCompleted) return Visibility.Visible;
        return isCompleted ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}