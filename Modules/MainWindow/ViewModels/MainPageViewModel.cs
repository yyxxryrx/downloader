using System;
using Windows.ApplicationModel.DataTransfer;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Downloader.Modules.MainWindow.Views;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;

namespace Downloader.Modules.MainWindow.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty] private string _headerText = "History";

        [ObservableProperty] private NavigationViewItem _item;

        [ObservableProperty] private Type _pageType;

        public void ItemChanged(NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                HeaderText = "Settings";
                PageType = typeof(SettingsPage);
                return;
            }

            HeaderText = (args.SelectedItemContainer.Tag as string)!;
            PageType = (args.SelectedItemContainer.Tag as string) switch
            {
                "History" => typeof(HistoryPage),
                "Downloading" => typeof(DownloadingPage),
                "Logging" => typeof(LoggingPage),
                _ => PageType
            };
        }

        [RelayCommand]
        private void NewDownload()
        {
            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            if (dispatcherQueue.HasThreadAccess)
                Show();
            else
                dispatcherQueue.TryEnqueue(Show);
            return;

            async void Show()
            {
                var package = Clipboard.GetContent();
                var url = "";
                if (package.Contains(StandardDataFormats.Text))
                {
                    try
                    {
                        url = new Uri(await package.GetTextAsync()).AbsoluteUri;
                    }
                    catch
                    {
                        // ignored
                    }
                }
                var askWindow = new AskWindow.AskWindow(url);
                askWindow.Activate();
            }
        }
    }
}