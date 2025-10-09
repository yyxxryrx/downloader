using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using BidirectionalMap;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Downloader.Modules.Dialogs.MessageBox;
using Downloader.Modules.MainWindow.Views;
using Downloader.Modules.Services;
using Downloader.Modules.Utils;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.Resources;
using Microsoft.Windows.Storage.Pickers;
using Serilog;

namespace Downloader.Modules.MainWindow.ViewModels;

public partial class SettingsPageViewModel : ObservableObject
{
    public BiMap<string, string> Language { get; } = new()
    {
        { new ResourceLoader().GetString("Auto"), "" },
        { "English", "en-US" },
        { "中文（简体）", "zh-CN" }
    };

    public List<DownloadPathMode> DownloadPathModes =
    [
        DownloadPathMode.Default,
        DownloadPathMode.Custom
    ];

    [ObservableProperty] private string _currentLanguage;
    [ObservableProperty] private string _path;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(DownloadPathModeIsCustom))]
    private DownloadPathMode _downloadPathMode;

    public bool DownloadPathModeIsCustom => DownloadPathMode is DownloadPathMode.Custom;

    private XamlRoot XamlRoot { get; }

    public SettingsPageViewModel(XamlRoot xamlRoot)
    {
        XamlRoot = xamlRoot;
        CurrentLanguage = Language.Reverse[GlobalVars.ConfigurationService.Language];
        DownloadPathMode = Enum.Parse<DownloadPathMode>(GlobalVars.ConfigurationService.DownloadPathMode);
        Path = GlobalVars.ConfigurationService.DownloadPath;
        if (
            string.IsNullOrEmpty(Path) &&
            GetDownloadFolderPath.GetPath() is { } path
        )
        {
            Path = path;
            GlobalVars.ConfigurationService.DownloadPath = Path;
        }
    }

    public void LanguageSelectChanged()
    {
        GlobalVars.ConfigurationService.Language = Language.Forward[CurrentLanguage];
    }

    [RelayCommand]
    private void OpenDirectoryPicker(Button button)
    {
        var picker = new FolderPicker(button.XamlRoot.ContentIslandEnvironment.AppWindowId)
        {
            CommitButtonText = "Select target folder",
            SuggestedStartLocation = PickerLocationId.Downloads,
        };
        button.DispatcherQueue.TryEnqueue(async void () =>
        {
            try
            {
                var path = await picker.PickSingleFolderAsync();
                if (path is null) return;
                Path = path.Path;
                await SavePath();
            }
            catch (Exception e)
            {
                Log.Error(e, "Pick folder error");
            }
        });
    }

    public async Task SavePath()
    {
        try
        {
            if (GlobalVars.ConfigurationService.DownloadPath == Path) return;
            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);
            GlobalVars.ConfigurationService.DownloadPath = Path;
            Log.Information("The download path change to {Path}", Path);
        }
        catch (Exception e)
        {
            Log.Error(e, "Save path error");
            var messageBox = new MessageBoxDialog
            {
                XamlRoot = XamlRoot,
                Message = e.Message,
                Icon = MessageIconType.Error,
                Button = MessageButtonType.Ok
            };
            await messageBox.ShowAsync();
        }
    }

    public async Task DownloadPathModeChanged()
    {
        if (DownloadPathMode is DownloadPathMode.Default && GetDownloadFolderPath.GetPath() is { } path)
        {
            Path = path;
            await SavePath();
        }
        GlobalVars.ConfigurationService.DownloadPathMode = DownloadPathMode.ToString();
    }
}

public enum DownloadPathMode
{
    Default,
    Custom
}