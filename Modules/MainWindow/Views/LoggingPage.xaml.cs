using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Downloader.Modules.MainWindow.ViewModels;
using Microsoft.UI;
using Microsoft.Windows.ApplicationModel.Resources;
using Serilog.Events;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Downloader.Modules.MainWindow.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LoggingPage : Page
{
    public LoggingPageViewModel ViewModel => (DataContext as LoggingPageViewModel)!;

    public LoggingPage()
    {
        InitializeComponent();
        DataContext = new LoggingPageViewModel();
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ViewModel.Selector_OnSelectionChanged(e);
    }

    private void LoggingPage_OnLoaded(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine(ViewModel.InitSelectItems);
        foreach (var level in ViewModel.InitSelectItems)
            TokenView1.SelectedItems.Add(level);
    }
}




