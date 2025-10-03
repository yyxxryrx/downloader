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
using Downloader.Modules.AskWindow.Models;
using Downloader.Modules.AskWindow.ViewModels;
using Downloader.Modules.AskWindow.Views;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Downloader.Modules.AskWindow
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AskWindow : WindowEx
    {
        public AskWindow(string url)
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            Frame1.Navigate(typeof(MainPage), new AskWindowNavigateParameter(url, Close));
        }
    }
}
