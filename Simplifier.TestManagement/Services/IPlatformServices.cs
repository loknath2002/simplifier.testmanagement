using System.Threading.Tasks;

namespace Simplifier.TestManagement.Services;

public interface IPlatformServices
{
    Task<string?> OpenFolderDialogAsync();
    
    bool CreateDirectory(string theFullPath);
}
