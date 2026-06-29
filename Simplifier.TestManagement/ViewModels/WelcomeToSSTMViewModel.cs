// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Simplifier.TestManagement.Models;
using Simplifier.TestManagement.Services;
namespace Simplifier.TestManagement.ViewModels;

public partial class WelcomeToSSTMViewModel : ViewModelBase
{
    private readonly IPlatformServices platformServices;
    private readonly ProjectDetailsViewModel myProjectDetailsViewModel;
    private readonly IViewNavigation myViewNavigation;

    [ObservableProperty]
    private string? _Greeting = "Welcome to Super Simple Test Management!";

    [ObservableProperty]
    private ObservableCollection<string> _RecentProjects = new ObservableCollection<string> { "Project A", "Project B", "Project C" };

    [ObservableProperty]
    private string? _SubGreeting = "A Markdown based test case management tool";

    private readonly ProjectCreationWizardViewModel myProjectCreationWizardViewModel;

    public WelcomeToSSTMViewModel()
    {

    }

    public WelcomeToSSTMViewModel(IViewNavigation theViewNavigation, IPlatformServices thePlatformServices, ProjectDetailsViewModel theProjectDetailsViewModel, ProjectCreationWizardViewModel theProjectCreationWizardViewModel)
    {
        myViewNavigation = theViewNavigation;
        platformServices = thePlatformServices;
        myProjectDetailsViewModel = theProjectDetailsViewModel;
        myProjectCreationWizardViewModel = theProjectCreationWizardViewModel;
    }

    [RelayCommand]
    public async Task CreateNewProject()
    {
        myViewNavigation.Navigate(this, myProjectCreationWizardViewModel);
    }

    private void CreateTestModelAndNavigate(string thePath)
    {
        var project = TestProjectModel.FromFullPath(thePath);
        if (!project.Validate())
        {
            return;
        }

        myViewNavigation.Navigate(this, myProjectDetailsViewModel);
        myProjectDetailsViewModel.LoadTests(project);
    }

    [RelayCommand]
    public async void OpenExistingProject()
    {
        string? theExistingPath = await platformServices.OpenFolderDialogAsync();
        if (string.IsNullOrEmpty(theExistingPath))
        {
            return;
        }

        CreateTestModelAndNavigate(theExistingPath);
    }
}