using System;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;

namespace Simplifier.TestManagement.Services;

public interface IPlatformServices
{
    Task<string?> OpenFolderDialogAsync();
    
    bool CreateDirectory(string theFullPath);

    Task<bool> ShowYesNoDialog(string theTitle, string theMessage, NotificationType theType);
}
