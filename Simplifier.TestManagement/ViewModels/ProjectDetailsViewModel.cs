// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using SukiUI.Toasts;
namespace Simplifier.TestManagement.ViewModels;

#region Using statements

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Simplifier.Serilog;
using Simplifier.TestManagement.Models;
using Simplifier.TestManagement.Services;

#endregion

public partial class ProjectDetailsViewModel : ViewModelBase
{
#region Fields

    private const int TestTabIndex = 1;

    private readonly LoggerDetail myLogDetail = new LoggerDetail(typeof(ProjectDetailsViewModel));
    private readonly ITestCaseRendererFactory myRendererFactory;
    private readonly ISharedContext mySharedContext;
    private readonly IStorageService myStorageService;

    [ObservableProperty]
    private ObservableCollection<TestStepResultModel> _ActiveRunSteps = new ObservableCollection<TestStepResultModel>();

    [ObservableProperty]
    private TestProjectModel? _CurrentProject;

    [ObservableProperty]
    private TestCaseModel? _SelectedRunTestCase;

    [ObservableProperty]
    private int _SelectedTabIndex;

    [ObservableProperty]
    private TestCaseModel? _SelectedTestCase;

    private readonly ISukiToastManager myToastManager;

    #endregion
#region Construction / Destruction

    public ProjectDetailsViewModel(
        ISukiToastManager theToastManager,
        IStorageService storageService, ITestCaseRendererFactory rendererFactory, ISharedContext sharedContext)
    {
        myToastManager = theToastManager;
        myStorageService = storageService;
        myRendererFactory = rendererFactory;
        mySharedContext = sharedContext;
    }

#endregion
#region Properties

    public bool IsAddStepVisible => SelectedTabIndex == TestTabIndex;

    public IEnumerable<TestCaseStatus> TestCaseStatuses => Enum.GetValues<TestCaseStatus>();

    public ObservableCollection<TestCaseModel> Tests { get; } = new ObservableCollection<TestCaseModel>();

    public IEnumerable<TestStepStatus> TestStepStatuses => Enum.GetValues<TestStepStatus>();

    #endregion
#region Members

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

    [RelayCommand]
    private void AddStep()
    {
        if (SelectedTestCase == null)
        {
            return;
        }

        SelectedTestCase.Steps.Add(new TestStepModel(SelectedTestCase.Steps.Count + 1, "", ""));
    }

    private void AddToVisibleAndInvisibleLists(TestCaseModel aNewTestCase)
    {
        if (!Tests.Contains(aNewTestCase))
        {
            Tests.Add(aNewTestCase);
        }

        mySharedContext.OriginalTestCases[aNewTestCase.UniqueId] = aNewTestCase.DeepCopy();
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

    partial void OnSelectedTabIndexChanged(int value)
    {
        OnPropertyChanged(nameof(ProjectDetailsViewModel.IsAddStepVisible));
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
    private void SaveRun()
    {
        using var aTrace = new LogScope(myLogDetail);
        TestCaseModel? aModel = SelectedRunTestCase;
        if (aModel == null || ActiveRunSteps.Count == 0)
        {
            aTrace.Warning("SaveRun aborted: no test case selected or no active steps.");
            return;
        }

        // Validate all step results – Note is required when Status is Fail
        bool aValid = true;
        foreach (TestStepResultModel result in ActiveRunSteps)
        {
            bool aStepValid = result.Validate();
            if (!aStepValid)
            {
                aTrace.Warning($"Step {result.StepNumber} validation failed: {string.Join("|", result.GetErrors().Select(x => x.ErrorMessage))}");
            }
            aValid &= aStepValid;
        }

        if (!aValid)
        {
            aTrace.Warning("SaveRun aborted: one or more step results failed validation.");
            return;
        }

        var aRun = new TestRun(DateOnly.FromDateTime(DateTime.Now));
        foreach (TestStepResultModel result in ActiveRunSteps)
        {
            aRun.Results.Add(result);
        }

        aModel.TestRuns.Add(aRun);
        StoreTestCase(aModel);
        OnSelectedRunTestCaseChanged(SelectedRunTestCase);
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
            aTrace.Warning("Test Steps has errors.");
            return;
        }

        if (StoreTestCase(aSelectedTestRef))
        {
            return;
        }
        
        AddToVisibleAndInvisibleLists(aSelectedTestRef);
        BuildIndexableIndex();
        myToastManager.CreateToast()
            .WithTitle("Saved")
            .WithContent("Test case saved successfully.")
            .Queue();
        aTrace.Info("Test case saved successfully.");
    }
    private bool StoreTestCase(TestCaseModel aSelectedTestRef)
    {
        using var aTrace = new LogScope(myLogDetail);
        ITestCaseRenderer renderer = myRendererFactory.Create(aSelectedTestRef);
        TestCaseTarget target = renderer.Write(aSelectedTestRef);
        if (!mySharedContext.OriginalTestCases.ContainsKey(aSelectedTestRef.UniqueId))
        {
            aTrace.Error($"Original test case not found: {aSelectedTestRef.UniqueId}");
            return true;
        }

        myStorageService.Save(mySharedContext.OriginalTestCases[aSelectedTestRef.UniqueId].StoragePath(), aSelectedTestRef.StoragePath(), target.Content);
        return false;
    }

    #endregion
}