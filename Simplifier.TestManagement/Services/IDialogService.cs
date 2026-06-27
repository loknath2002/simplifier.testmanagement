using System.Threading.Tasks;

namespace Simplifier.TestManagement.Services;

public interface IDialogService
{
    Task<string?> OpenFolderDialogAsync();
}
