// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System;
using CommunityToolkit.Mvvm.Input;
using Simplifier.TestManagement.Services;
namespace Simplifier.TestManagement.ViewModels;

public partial class WelcomeToSSTMViewModel(IViewNavigation myViewNavigation, ProjectDetailsViewModel myProjectDetailsViewModel) : ViewModelBase
{
    public string Greeting => "Welcome to Super Simple Test Management Console!";

    [RelayCommand]
    public void CreateNewProject()
    {
        myViewNavigation.Navigate(this, myProjectDetailsViewModel);
    }
    
    [RelayCommand]
    public void OpenExistingProject()
    {
        
    }
}