using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Downloader.Modules.MainWindow.Views;
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
    }
}