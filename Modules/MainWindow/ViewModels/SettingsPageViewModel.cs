using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BidirectionalMap;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Downloader.Modules.MainWindow.Views;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.Resources;

namespace Downloader.Modules.MainWindow.ViewModels
{
    public partial class SettingsPageViewModel : ObservableObject
    {
        public BiMap<string, string> Language { get; } = new()
        {
            { new ResourceLoader().GetString("Auto"), "" },
            { "English", "en-US" },
            { "中文（简体）", "zh-CN" }
        };

        [ObservableProperty] private string _currentLanguage;

        public SettingsPageViewModel()
        {
            CurrentLanguage = Language.Reverse[GlobalVars.ConfigurationService.Language];
        }

        public void LanguageSelectChanged()
        {
            GlobalVars.ConfigurationService.Language = Language.Forward[CurrentLanguage];
        }
    }
}