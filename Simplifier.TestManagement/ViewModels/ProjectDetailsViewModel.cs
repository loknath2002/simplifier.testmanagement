// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Simplifier.TestManagement.Models;
using Simplifier.TestManagement.Services;

namespace Simplifier.TestManagement.ViewModels;

public partial class ProjectDetailsViewModel : ViewModelBase
{
    private readonly IStorageService _storageService;

    [ObservableProperty]
    private ObservableCollection<TestCaseModel> _tests = new();

    [ObservableProperty]
    private TestProjectModel? _currentProject;

    [ObservableProperty]
    private TestCaseModel? _selectedTestCase;

    public ProjectDetailsViewModel(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public void LoadTests(TestProjectModel project)
    {
        CurrentProject = project;
        Tests = new ObservableCollection<TestCaseModel>(_storageService.Load(project.DirectoryPath));
    }

    [RelayCommand]
    private void CreateNewTestCase()
    {
        // Implementation for creating a new test case
    }

    [RelayCommand]
    private void SaveTestCase()
    {
        // Implementation for saving a test case
    }

    [RelayCommand]
    private void DeleteTestCase()
    {
        // Implementation for deleting a test case
    }
}