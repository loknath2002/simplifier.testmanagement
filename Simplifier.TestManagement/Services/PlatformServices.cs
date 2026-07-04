using System.IO;
using System.Linq;
using System;
using System.Threading.Tasks;
using SukiUI.Dialogs;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;

namespace Simplifier.TestManagement.Services;

public class PlatformServices : IPlatformServices
{
    private readonly ISukiDialogManager theDialogManager;

    public PlatformServices(ISukiDialogManager theDialogManager)
    {
        this.theDialogManager = theDialogManager;
    }
    
    public async Task<string?> OpenFolderDialogAsync()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var topLevel = TopLevel.GetTopLevel(desktop.MainWindow);
            if (topLevel != null)
            {
                var folder = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()).ConfigureAwait(false);
                return folder.FirstOrDefault()?.TryGetLocalPath();
            }
        }
        
        return null;
    }
    public bool CreateDirectory(string theFullPath)
    {
        try
        {
            if (Directory.Exists(theFullPath))
            {
                return false;
            }

            Directory.CreateDirectory(theFullPath);
            return true;
        }
        catch
        {
            return false;   
        }
    }

    public Task<bool> ShowYesNoDialog(string theTitle, string message, NotificationType theType)
    {
        var tcs = new TaskCompletionSource<bool>();
        var result = theDialogManager.CreateDialog()
            .WithTitle(theTitle)
            .WithContent(message)
            .Dismiss()
            .ByClickingBackground()
            .OfType(theType)
            .WithActionButton("Yes", _ => tcs.TrySetResult(true))
            .WithActionButton("No", _ => tcs.TrySetResult(false), true)
            .TryShow();
        
        if (!result)
        {
            tcs.TrySetResult(false);
        }
        
        return tcs.Task;
    }
}
