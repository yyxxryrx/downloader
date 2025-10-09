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
using Microsoft.Windows.ApplicationModel.Resources;
using Serilog;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Downloader.Modules.MainWindow.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SettingsPage : Page
{
    public SettingsPageViewModel ViewModel => (DataContext as SettingsPageViewModel)!;

    public SettingsPage()
    {
        InitializeComponent();
        DataContext = new SettingsPageViewModel();
    }

    private void Language_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.RemovedItems.Count == 0) return;
        ViewModel.LanguageSelectChanged();
        InfoBar1.IsOpen = true;
    }

    private async void PathTextBox_OnLostFocus(object sender, RoutedEventArgs e)
    {
        var textBox = (sender as TextBox)!;
        Log.Debug("TextBox Lost Focus with value: {Text}", textBox.Text);
        ViewModel.Path = textBox.Text;
        await ViewModel.SavePath();
    }

    private async void DownloadPathMode_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        await ViewModel.DownloadPathModeChanged();
    }
}

public class EnumConvertToString : IValueConverter
{
    private ResourceLoader Loader { get; } = new();

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not Enum enumValue) return "";
        try
        {
            return Loader.GetString(enumValue.ToString());
        }
        catch
        {
            return "Not found";
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}