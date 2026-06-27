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
    private readonly ITestCaseRendererFactory _rendererFactory;

    [ObservableProperty]
    private ObservableCollection<TestCaseModel> _tests = new();

    [ObservableProperty]
    private TestProjectModel? _currentProject;

    [ObservableProperty]
    private TestCaseModel? _selectedTestCase;

    public ProjectDetailsViewModel(IStorageService storageService, ITestCaseRendererFactory rendererFactory)
    {
        _storageService = storageService;
        _rendererFactory = rendererFactory;
    }

    public void LoadTests(TestProjectModel project)
    {
        CurrentProject = project;
        Tests = new ObservableCollection<TestCaseModel>(_storageService.Load(project.DirectoryPath));
    }

    [RelayCommand]
    private void CreateNewTestCase()
    {
        this.SelectedTestCase = new TestCaseModel(CurrentProject, new TestCaseMetadataModel());
    }

    [RelayCommand]
    private void SaveTestCase()
    {
        if (SelectedTestCase == null) return;
        var path = SelectedTestCase.StoragePath();
        var renderer = _rendererFactory.Create(SelectedTestCase);
        var target = renderer.Write(SelectedTestCase);
        _storageService.Save(path, target.Content);
    }

    [RelayCommand]
    private void DeleteTestCase()
    {
        // Implementation for deleting a test case
    }
}