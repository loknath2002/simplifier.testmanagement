using Avalonia.Controls.Notifications;
using SukiUI.Toasts;

namespace Simplifier.TestManagement.Services;

public class SukiToastMessenger(ISukiToastManager myToastManager) : IToastMessenger
{

    public void Info(string theTitle, string theMessage)
    {
        myToastManager.CreateToast().WithTitle(theTitle).WithContent(theMessage).OfType(NotificationType.Information).Queue();
    }

    public void Error(string theTitle, string theMessage)
    {
        myToastManager.CreateToast().WithTitle(theTitle).WithContent(theMessage).OfType(NotificationType.Error).Queue();
    }

    public void Warning(string theTitle, string theMessage)
    {
        myToastManager.CreateToast().WithTitle(theTitle).WithContent(theMessage).OfType(NotificationType.Warning).Queue();
    }

    public void Success(string theTitle, string theMessage)
    {
        myToastManager.CreateToast().WithTitle(theTitle).WithContent(theMessage).OfType(NotificationType.Success).Queue();
    }
}
