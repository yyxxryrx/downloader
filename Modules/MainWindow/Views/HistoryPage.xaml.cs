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
using Windows.Globalization.NumberFormatting;
using Downloader.Modules.Utils;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Downloader.Modules.MainWindow.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class HistoryPage : Page
{
    public HistoryPage()
    {
        InitializeComponent();
        DispatcherQueue.TryEnqueue(async void () => { await GlobalVars.HistoryManagerService.Initialize(); });
    }
}

public class FileSizeFormater : INumberFormatter
{
    public string Format(long value)
    {
        return FilesizeConverters.iB.Convert(value).ToStringF2();
    }

    public string Format(ulong value)
    {
        return FilesizeConverters.iB.Convert((long)value).ToStringF2();
    }

    public string Format(double value)
    {
        return FilesizeConverters.iB.Convert((long)value).ToStringF2();
    }
}