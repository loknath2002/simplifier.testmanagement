using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace Simplifier.TestManagement.Services;

public class PlatformServices : IPlatformServices
{
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
}
