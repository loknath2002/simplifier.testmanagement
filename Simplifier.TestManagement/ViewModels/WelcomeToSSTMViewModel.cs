// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Simplifier.TestManagement.Models;
using Simplifier.TestManagement.Properties;
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
    private ObservableCollection<TestProjectModel> _RecentProjects = new ObservableCollection<TestProjectModel>();

    [ObservableProperty]
    private TestProjectModel? _SelectedRecentProject;

    [ObservableProperty]
    private string? _SubGreeting = "An AI friendly Markdown based test management tool";

    private readonly ProjectCreationWizardViewModel myProjectCreationWizardViewModel;

    public WelcomeToSSTMViewModel(IViewNavigation theViewNavigation, IPlatformServices thePlatformServices, ProjectDetailsViewModel theProjectDetailsViewModel, ProjectCreationWizardViewModel theProjectCreationWizardViewModel)
    {
        myViewNavigation = theViewNavigation;
        platformServices = thePlatformServices;
        myProjectDetailsViewModel = theProjectDetailsViewModel;
        myProjectCreationWizardViewModel = theProjectCreationWizardViewModel;
        LoadRecentProjectsFromSettings();
    }

    // ------------------------------------------------------------------ navigation

    [RelayCommand]
    public async Task CreateNewProject()
    {
        myViewNavigation.Navigate(this, myProjectCreationWizardViewModel);
    }

    [RelayCommand]
    public async Task OpenExistingProject()
    {
        string? theExistingPath = await platformServices.OpenFolderDialogAsync();
        if (string.IsNullOrEmpty(theExistingPath))
        {
            return;
        }

        AddAndSaveRecentProject(theExistingPath);
        NavigateToProject(TestProjectModel.FromFullPath(theExistingPath));
    }

    [RelayCommand]
    public void OpenRecentProject(TestProjectModel project)
    {
        AddAndSaveRecentProject(project.ProjectPath);
        NavigateToProject(project);
    }

    partial void OnSelectedRecentProjectChanged(TestProjectModel? value)
    {
        if (value is null)
        {
            return;
        }
        
        OpenRecentProjectCommand.Execute(value);
        // Clear selection so re-selecting the same entry works next time
        SelectedRecentProject = null;
    }

    private void NavigateToProject(TestProjectModel project)
    {
        if (!project.Validate())
        {
            return;
        }

        myProjectDetailsViewModel.LoadTests(project);
        myViewNavigation.Navigate(this, myProjectDetailsViewModel);
    }

    private void LoadRecentProjectsFromSettings()
    {
        RecentProjects.Clear();
        StringCollection? saved = Settings.Default.RecentProjects;
        if (saved == null) return;

        foreach (string? entry in saved)
        {
            if (!string.IsNullOrWhiteSpace(entry) && Directory.Exists(entry))
            {
                RecentProjects.Add(TestProjectModel.FromFullPath(entry));
            }
        }
    }

    internal static void AddAndSaveRecentProject(string path)
    {
        StringCollection collection = Settings.Default.RecentProjects ?? new StringCollection();

        // Remove existing occurrence so we can move it to the front
        collection.Remove(path);
        collection.Insert(0, path);

        int max = Settings.Default.MaxRecentProjects > 0 ? Settings.Default.MaxRecentProjects : 10;
        while (collection.Count > max)
        {
            collection.RemoveAt(collection.Count - 1);
        }

        Settings.Default.RecentProjects = collection;
        Settings.Default.Save();
    }
}