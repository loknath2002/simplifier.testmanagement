// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarfBuzzSharp;
using Simplifier.TestManagement.Models;
using Simplifier.TestManagement.Services;
namespace Simplifier.TestManagement.ViewModels;

public partial class ProjectCreationWizardViewModel : ViewModelBase
{
    private readonly IPlatformServices myplatformServices;
    
    private readonly IViewNavigation myNavigation;

    private readonly ProjectDetailsViewModel myProjectDetailsViewModel;

    [ObservableProperty]
    private TestProjectModel _Project = new TestProjectModel();

    private readonly ISharedContext mySharedContext;
    private readonly IToastMessenger myToastManager;

    public ProjectCreationWizardViewModel(ISharedContext theSharedContext, IPlatformServices thePatformServices, IViewNavigation theNavigation, ProjectDetailsViewModel theProjectDetailsViewModel, IToastMessenger theToastMessenger)
    {
        mySharedContext = theSharedContext;
        myplatformServices = thePatformServices;
        myNavigation = theNavigation;
        myProjectDetailsViewModel = theProjectDetailsViewModel;
        myToastManager = theToastMessenger;
    }

    [RelayCommand]
    private async Task PickFolder()
    {
        string? aDirectory = await myplatformServices.OpenFolderDialogAsync().ConfigureAwait(false);
        if (aDirectory != null)
        {
            Project.DirectoryPath = aDirectory;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        myNavigation.NavigateBack(this);
    }

    [RelayCommand]
    private void Proceed()
    {
        if (!Project.Validate())
        {
            return;
        }

        if (!myplatformServices.CreateDirectory(Project.ProjectPath))
        {
            myToastManager.Error("Creation Failed", "A project with the same name already exists.");
            return;
        }

        myToastManager.Success("Project Created", "Project created successfully.");
        WelcomeToSSTMViewModel.AddAndSaveRecentProject(Project.ProjectPath);
        myProjectDetailsViewModel.LoadTests(Project);
        myNavigation.Navigate(this, myProjectDetailsViewModel);
    }
}