using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Downloader.Modules.Dialogs.MessageBox;

public class MessageBoxDialog()
{
    public string Message = string.Empty;
    public string Caption = string.Empty;
    public string SubTitle = string.Empty;
    public MessageIconType Icon = MessageIconType.None;
    public MessageButtonType Button = MessageButtonType.Ok;

    public async Task ShowAsync()
    {
        var message = string.Join('\n', from line in Message.Split('\n') select line.TrimEnd()); 
        var content = new MessageBoxDialogContent
        {
            Message = message,
            Icon = Icon,
            SubTitle = SubTitle
        };
        var dialog = new ContentDialog
        {
            XamlRoot = App.CurrentWindow!.Content.XamlRoot,
            Title = Caption,
            Content = content
        };
        switch (Button)
        {
            case MessageButtonType.Ok:
                dialog.CloseButtonText = "OK";
                dialog.DefaultButton = ContentDialogButton.Close;
                break;
            case MessageButtonType.OkCancel:
                dialog.PrimaryButtonText = "OK";
                dialog.CloseButtonText = "Cancel";
                dialog.DefaultButton = ContentDialogButton.Primary;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(Button), Button, null);
        }

        await dialog.ShowAsync();
    }
}

public enum MessageButtonType
{
    Ok,
    OkCancel,
}

public enum MessageIconType
{
    None = 0,
    Error,
    Warning,
    Information,
}