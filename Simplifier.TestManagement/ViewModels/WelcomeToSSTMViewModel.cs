// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Simplifier.TestManagement.Models;
using Simplifier.TestManagement.Services;
namespace Simplifier.TestManagement.ViewModels;

public partial class WelcomeToSSTMViewModel : ViewModelBase
{
    private readonly IDialogService myDialogService;
    private readonly ProjectDetailsViewModel myProjectDetailsViewModel;
    private readonly IViewNavigation myViewNavigation;

    [ObservableProperty]
    private string? _Greeting = "Welcome to Super Simple Test Management Console!";

    public WelcomeToSSTMViewModel()
    {

    }

    public WelcomeToSSTMViewModel(IViewNavigation theViewNavigation, IDialogService theDialogService, ProjectDetailsViewModel theProjectDetailsViewModel)
    {
        myViewNavigation = theViewNavigation;
        myDialogService = theDialogService;
        myProjectDetailsViewModel = theProjectDetailsViewModel;
    }

    [RelayCommand]
    public async Task CreateNewProject()
    {
        string? path = await myDialogService.OpenFolderDialogAsync();
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        CreateTestModelAndNavigate(path);
    }
    
    private void CreateTestModelAndNavigate(string path)
    {
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