// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        CurrentProject = project;
        Tests = new ObservableCollection<TestCaseModel>(myStorageService.Load(project.DirectoryPath));
        BuildIndexableIndex();
    }
    
    private void BuildIndexableIndex()
    {
        
    }

    [RelayCommand]
    private void CreateNewTestCase()
    {
        if (CurrentProject == null)
        {
            return;
        }
        
        var aNewTestCase = new TestCaseModel(CurrentProject, new TestCaseMetadataModel());
        {
            aNewTestCase.MetaData.Revision++;
            aNewTestCase.MetaData.Date = DateOnly.FromDateTime(DateTime.Now);
            aNewTestCase.MetaData.Status = TestCaseStatus.Draft;
        }

        IEnumerable<TestCaseModel> aTestIds = Tests.Where(x => x.Id > 0).ToList();
        if (!aTestIds.Any())
        {
            aNewTestCase.Id = 1;
        }
        else
        {
            aNewTestCase.Id = aTestIds.Max(x => x.Id) + 1;
        }

        Tests.Add(aNewTestCase);
        SelectedTestCase = aNewTestCase;
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