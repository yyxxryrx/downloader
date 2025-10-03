using System;
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

            void Show()
            {
                var askWindow = new AskWindow.AskWindow("https://dl.hdslb.com/mobile/fixed/bili_win/bili_win-install.exe?v=1.17.2-4&spm_id_from=..0.0");
                askWindow.Activate();
            }
        }
    }
}