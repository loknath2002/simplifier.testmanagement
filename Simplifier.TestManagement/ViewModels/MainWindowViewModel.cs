using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Simplifier.TestManagement.Services;
namespace Simplifier.TestManagement.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IViewNavigation
{
    [ObservableProperty]
    private ViewModelBase _CurrentPage;

    private readonly Stack<ViewModelBase> _history = new();

    public void Navigate(ViewModelBase? theFrom, ViewModelBase theTo)
    {
        if (theFrom != null)
        {
            _history.Push(theFrom);
        }
        (this.CurrentPage as IDisposable)?.Dispose();
        this.CurrentPage = theTo;
    }

    public void NavigateBack(ViewModelBase theFrom)
    {
        if (_history.Count > 0)
        {
            var previous = _history.Pop();
            (this.CurrentPage as IDisposable)?.Dispose();
            this.CurrentPage = previous;
        }
    }
}