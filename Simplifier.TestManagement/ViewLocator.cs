using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Simplifier.TestManagement.ViewModels;
namespace Simplifier.TestManagement;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? theParam)
    {
        if (theParam is null)
            return null;

        string aViewModelName = theParam.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var aViewModelType = Type.GetType(aViewModelName);

        if (aViewModelType != null)
        {
            return (Control)Activator.CreateInstance(aViewModelType)!;
        }

        return new TextBlock
        {
            Text = "Not Found: " + aViewModelName,
        };
    }

    public bool Match(object? theInstanceToMatch)
    {
        return theInstanceToMatch is ViewModelBase;
    }
}