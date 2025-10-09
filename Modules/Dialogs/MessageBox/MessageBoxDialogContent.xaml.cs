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
using Windows.UI.Text;
using Microsoft.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Downloader.Modules.Dialogs.MessageBox
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MessageBoxDialogContent : Page
    {
        public string Message;
        public MessageIconType Icon = MessageIconType.None;
        #nullable enable
        public string? SubTitle = null;
        public double? TextFontSize = null;
        public Brush? ForegroundColor = null;
        public FontWeight? TextFontWeight = null;
        #nullable disable

        public MessageBoxDialogContent()
        {
            InitializeComponent();
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            TextBlock1.Text = Message;
            if (SubTitle != null)
                SubTitleBlock.Text = SubTitle;
            else
            {
                Grid1.Children.Remove(SubTitleBlock);
                Grid1.RowDefinitions.RemoveAt(0);
                Grid.SetRow(TextBlock1, 0);
                Grid.SetRow(FontIcon1, 0);
            }
            if (TextFontSize != null)
                TextBlock1.FontSize = TextFontSize.Value;
            if (TextFontWeight != null)
                TextBlock1.FontWeight = TextFontWeight.Value;
            if (ForegroundColor != null)
                TextBlock1.Foreground = ForegroundColor;
            switch (Icon)
            {
                case MessageIconType.Error:
                    FontIcon1.Glyph = "\uEB90";
                    FontIcon1.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    break;
                case MessageIconType.Warning:
                    FontIcon1.Glyph = "\uE814";
                    FontIcon1.Foreground = new SolidColorBrush(Colors.Yellow);
                    break;
                case MessageIconType.Information:
                    FontIcon1.Glyph = "\uF167";
                    FontIcon1.Foreground = new SolidColorBrush(Colors.DodgerBlue);
                    break;
                case MessageIconType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Icon), Icon, null);
            }
        }
    }
}
