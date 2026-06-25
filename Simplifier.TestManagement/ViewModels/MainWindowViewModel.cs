using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Simplifier.TestManagement.Services;
namespace Simplifier.TestManagement.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IViewNavigation
{
    [ObservableProperty]
    private ViewModelBase _CurrentPage;

    public void Navigate(ViewModelBase theFrom, ViewModelBase theTo)
    {
        (this.CurrentPage as IDisposable)?.Dispose();
        this.CurrentPage = theTo;
    }
}