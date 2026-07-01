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
    private const int TestTabIndex = 1;
    private readonly ITestCaseRendererFactory myRendererFactory;
    private readonly ISharedContext mySharedContext;
    private readonly IStorageService myStorageService;

    [ObservableProperty]
    private TestProjectModel? _CurrentProject;

    [ObservableProperty]
    private int _SelectedTabIndex;

    [ObservableProperty]
    private TestCaseModel? _SelectedTestCase;

    [ObservableProperty]
    private TestCaseModel? _SelectedRunTestCase;

    [ObservableProperty]
    private ObservableCollection<TestStepResultModel> _ActiveRunSteps = new ObservableCollection<TestStepResultModel>();

    private readonly LoggerDetail myLogDetail = new LoggerDetail(typeof(ProjectDetailsViewModel));

    public ProjectDetailsViewModel(IStorageService storageService, ITestCaseRendererFactory rendererFactory, ISharedContext sharedContext)
    {
        myStorageService = storageService;
        myRendererFactory = rendererFactory;
        mySharedContext = sharedContext;
    }

    public bool IsAddStepVisible => SelectedTabIndex == TestTabIndex;

    public ObservableCollection<TestCaseModel> Tests { get; } = new ObservableCollection<TestCaseModel>();

    public IEnumerable<TestCaseStatus> TestCaseStatuses => Enum.GetValues<TestCaseStatus>();
    
    public IEnumerable<TestStepStatus> TestStepStatuses => Enum.GetValues<TestStepStatus>();

    partial void OnSelectedTabIndexChanged(int value)
    {
        OnPropertyChanged(nameof(ProjectDetailsViewModel.IsAddStepVisible));
    }

    public void LoadTests(TestProjectModel project)
    {
        mySharedContext.OriginalTestCases.Clear();
        Tests.Clear();
        CurrentProject = project;
        foreach (TestCaseModel aModel in myStorageService.Load(project.ProjectPath))
        {
            AddToVisibleAndInvisibleLists(aModel);
        }

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

        AddToVisibleAndInvisibleLists(aNewTestCase);
        SelectedTestCase = aNewTestCase;
    }

    private void AddToVisibleAndInvisibleLists(TestCaseModel aNewTestCase)
    {
        if (!Tests.Contains(aNewTestCase))
        {
            Tests.Add(aNewTestCase);
        }

        mySharedContext.OriginalTestCases[aNewTestCase.UniqueId] = aNewTestCase.DeepCopy();
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

        for (int i = 0; i < SelectedTestCase.Steps.Count; i++)
        {
            SelectedTestCase.Steps[i].Number = i + 1;
        }
    }

    [RelayCommand]
    private void SaveTestCase()
    {
        using var aTrace = new LogScope(myLogDetail);
        TestCaseModel? aSelectedTestRef = SelectedTestCase;
        if (aSelectedTestRef == null)
        {
            return;
        }

        bool aStepsValid = true;
        foreach (TestStepModel aStep in aSelectedTestRef.Steps)
        {
            aStepsValid &= aStep.Validate();
        }
        
        if (!aSelectedTestRef.Validate())
        {
            aTrace.Warning($"Test input has errors, {string.Join("|", aSelectedTestRef.GetErrors().Select(x => x.ErrorMessage))}");
            return;
        }

        if (!aStepsValid)
        {
            SelectedTabIndex = TestTabIndex;
            aTrace.Warning($"Test Steps has errors.");
            return;
        }
        
        ITestCaseRenderer renderer = myRendererFactory.Create(aSelectedTestRef);
        TestCaseTarget target = renderer.Write(aSelectedTestRef);
        if (!mySharedContext.OriginalTestCases.ContainsKey(aSelectedTestRef.UniqueId))
        {
            aTrace.Error($"Original test case not found: {aSelectedTestRef.UniqueId}");
            return;
        }

        myStorageService.Save(mySharedContext.OriginalTestCases[aSelectedTestRef.UniqueId].StoragePath(), aSelectedTestRef.StoragePath(), target.Content);
        AddToVisibleAndInvisibleLists(aSelectedTestRef);
        BuildIndexableIndex();
    }

    [RelayCommand]
    private void DeleteTestCase()
    {
        // Implementation for deleting a test case
    }

    partial void OnSelectedRunTestCaseChanged(TestCaseModel? value)
    {
        ActiveRunSteps.Clear();
        if (value == null) return;
        foreach (TestStepModel step in value.Steps)
        {
            ActiveRunSteps.Add(new TestStepResultModel(step.Number, TestStepStatus.Pending)
            {
                Procedure = step.Procedure,
                ExpectedResult = step.ExpectedResult
            });
        }
    }

    [RelayCommand]
    private void SaveRun()
    {
        if (SelectedRunTestCase == null || ActiveRunSteps.Count == 0) return;

        var run = new TestRun(DateOnly.FromDateTime(DateTime.Now));
        foreach (TestStepResultModel result in ActiveRunSteps)
        {
            run.Results.Add(result);
        }
        SelectedRunTestCase.TestRuns.Add(run);

        // Re-initialize steps for a fresh run
        OnSelectedRunTestCaseChanged(SelectedRunTestCase);
    }
}