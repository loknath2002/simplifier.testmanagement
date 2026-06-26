// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Simplifier.TestManagement.Services;
namespace Simplifier.TestManagement.ViewModels;

public partial class WelcomeToSSTMViewModel : ViewModelBase
{
    private readonly IViewNavigation myViewNavigation;
    
    private readonly ProjectDetailsViewModel myProjectDetailsViewModel;
    
    [ObservableProperty]
    private string? _Greeting = "Welcome to Super Simple Test Management Console!";

    public WelcomeToSSTMViewModel()
    {
        
    }
    
    public WelcomeToSSTMViewModel(IViewNavigation theViewNavigation, ProjectDetailsViewModel theProjectDetailsViewModel)
    {
        this.myViewNavigation = theViewNavigation;
        this.myProjectDetailsViewModel = theProjectDetailsViewModel;
    }
    
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