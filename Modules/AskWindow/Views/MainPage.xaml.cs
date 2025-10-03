using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using Downloader.Modules.AskWindow.Models;
using Downloader.Modules.AskWindow.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Downloader.Modules.AskWindow.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel => (DataContext as MainPageViewModel)!;

        public MainPage()
        {
            InitializeComponent();
            DataContext = new MainPageViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is not AskWindowNavigateParameter parameter) return;
            ViewModel.CloseWindow = parameter.CloseWindow;
            try
            {
                ViewModel.LoadUri(new Uri(parameter.Url));
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    XamlRoot = XamlRoot,
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK"
                };
                dialog.ShowAsync();
            }
        }
    }
}