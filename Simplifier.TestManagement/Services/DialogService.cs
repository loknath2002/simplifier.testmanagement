using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace Simplifier.TestManagement.Services;

public class DialogService : IDialogService
{
    public async Task<string?> OpenFolderDialogAsync()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var topLevel = TopLevel.GetTopLevel(desktop.MainWindow);
            if (topLevel != null)
            {
                var folder = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions());
                return folder.FirstOrDefault()?.Path.LocalPath;
            }
        }
        
        return null;
    }
}
