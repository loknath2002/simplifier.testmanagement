// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Simplifier.TestManagement.Services;
using Simplifier.TestManagement.Models;

namespace Simplifier.TestManagement.ViewModels;

public partial class WelcomeToSSTMViewModel : ViewModelBase
{
    private readonly IViewNavigation myViewNavigation;
    private readonly IDialogService myDialogService;
    private readonly ProjectDetailsViewModel myProjectDetailsViewModel;
    
    [ObservableProperty]
    private string? _Greeting = "Welcome to Super Simple Test Management Console!";

    public WelcomeToSSTMViewModel()
    {
        
    }
    
    public WelcomeToSSTMViewModel(IViewNavigation theViewNavigation, IDialogService theDialogService, ProjectDetailsViewModel theProjectDetailsViewModel)
    {
        this.myViewNavigation = theViewNavigation;
        this.myDialogService = theDialogService;
        this.myProjectDetailsViewModel = theProjectDetailsViewModel;
    }
    
    [RelayCommand]
    public async Task CreateNewProject()
    {
        var path = await myDialogService.OpenFolderDialogAsync();
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        var project = new TestProjectModel(path);
        if (!project.Validate())
        {
            return;
        }

        myViewNavigation.Navigate(this, myProjectDetailsViewModel);
        myProjectDetailsViewModel.LoadTests(project);
    }
    
    [RelayCommand]
    public void OpenExistingProject()
    {
        
    }
}