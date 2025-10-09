using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.System.UserProfile;
using Downloader.Modules;
using Downloader.Modules.Services;
using Downloader.Modules.Utils;
using Serilog;
using ApplicationLanguages = Microsoft.Windows.Globalization.ApplicationLanguages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Downloader;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private Window? _window;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        if (!string.IsNullOrEmpty(GlobalVars.ConfigurationService.Language))
            ApplicationLanguages.PrimaryLanguageOverride = GlobalVars.ConfigurationService.Language;
        else if (GlobalizationPreferences.Languages.Count > 0 &&
                 ApplicationLanguages.Languages.Contains(GlobalizationPreferences.Languages[0]))
            ApplicationLanguages.PrimaryLanguageOverride = GlobalizationPreferences.Languages[0];
        else
            ApplicationLanguages.PrimaryLanguageOverride = "en-US";
        var configuration = new LoggerConfiguration();
        configuration
#if !DEBUG
            .MinimumLevel.Information()
#else
            .MinimumLevel.Debug()
#endif
            .WriteTo.Console()
            .WriteTo.Sink(GlobalVars.UiSink)
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30);
        Log.Logger = configuration.CreateLogger();
        Log.Information("Program started");

        if (string.IsNullOrEmpty(GlobalVars.ConfigurationService.DownloadPath) &&
            GetDownloadFolderPath.GetPath() is { } path)
            GlobalVars.ConfigurationService.DownloadPath = path;
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }
}