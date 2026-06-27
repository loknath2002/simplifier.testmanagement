// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Simplifier.Serilog;
using Simplifier.TestManagement.Models;
using Simplifier.TestManagement.Services;
namespace Simplifier.TestManagement.ViewModels;

public partial class ProjectDetailsViewModel : ViewModelBase
{
    private readonly ITestCaseRendererFactory myRendererFactory;
    private readonly IStorageService myStorageService;

    [ObservableProperty]
    private TestProjectModel? _CurrentProject;

    [ObservableProperty]
    private TestCaseModel? _SelectedTestCase;

    [ObservableProperty]
    private ObservableCollection<TestCaseModel> _Tests = new ObservableCollection<TestCaseModel>();
    
    [ObservableProperty]
    private ObservableCollection<string> _AllTestAuthors = new ObservableCollection<string>();

    private LoggerDetail myLogDetail = new LoggerDetail(typeof(ProjectDetailsViewModel));

    public ProjectDetailsViewModel(IStorageService storageService, ITestCaseRendererFactory rendererFactory)
    {
        myStorageService = storageService;
        myRendererFactory = rendererFactory;
    }

    public IEnumerable<TestCaseStatus> TestCaseStatuses => Enum.GetValues<TestCaseStatus>();

    public void LoadTests(TestProjectModel project)
    {
        Tests.Clear();
        AllTestAuthors.Clear();
        CurrentProject = project;
        Tests = new ObservableCollection<TestCaseModel>(myStorageService.Load(project.DirectoryPath));
        BuildIndexableIndex();
    }
    
    private void BuildIndexableIndex()
    {
        foreach (var aAuthor in Tests.Where(x => !string.IsNullOrWhiteSpace(x.MetaData.Author)).Select(X => X.MetaData.Author))
        {
            if (!AllTestAuthors.Contains(aAuthor))
            {
                AllTestAuthors.Add(aAuthor);
            }
        }
    }

    [RelayCommand]
    private void CreateNewTestCase()
    {
        if (CurrentProject == null)
        {
            return;
        }
        
        SelectedTestCase = new TestCaseModel(CurrentProject, new TestCaseMetadataModel());
        // Setup metadata
        {
            SelectedTestCase.MetaData.Revision++;
            SelectedTestCase.MetaData.Date = DateOnly.FromDateTime(DateTime.Now);
            SelectedTestCase.MetaData.Status = TestCaseStatus.Draft;
        }

        IEnumerable<TestCaseModel> aTestIds = Tests.Where(x => x.Id > 0).ToList();
        if (!aTestIds.Any())
        {
            SelectedTestCase.Id = 1;
        }
        else
        {
            SelectedTestCase.Id = aTestIds.Max(x => x.Id) + 1;
        }
        
        Tests.Add(SelectedTestCase);
    }

    [RelayCommand]
    private void AddStep()
    {
        if (SelectedTestCase == null)
        {
            return;
        }
        
        SelectedTestCase.Steps.Add(new TestStepModel(SelectedTestCase.Steps.Count + 1, "", ""));
    }

    [RelayCommand]
    private void RemoveStep(TestStepModel step)
    {
        if (SelectedTestCase == null)
        {
            return;
        }
        
        SelectedTestCase.Steps.Remove(step);
    }

    [RelayCommand]
    private void SaveTestCase()
    {
        using var aTrace = new LogScope(myLogDetail);
        if (SelectedTestCase == null)
        {
            return;
        }

        if (!SelectedTestCase.Validate())
        {
            aTrace.Warning($"Test input has errors, {string.Join("|", SelectedTestCase.GetErrors().Select(x => x.ErrorMessage))}");
            return;
        }

        string aFinalPath = SelectedTestCase.StoragePath();
        ITestCaseRenderer renderer = myRendererFactory.Create(SelectedTestCase);
        TestCaseTarget target = renderer.Write(SelectedTestCase);
        myStorageService.Save(aFinalPath, target.Content);
        BuildIndexableIndex();
    }

    [RelayCommand]
    private void DeleteTestCase()
    {
        // Implementation for deleting a test case
    }
}