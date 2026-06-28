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
    private readonly ISharedContext mySharedContext;

    [ObservableProperty]
    private TestProjectModel? _CurrentProject;

    [ObservableProperty]
    private TestCaseModel? _SelectedTestCase;

    public ObservableCollection<TestCaseModel> Tests { get; } = new ObservableCollection<TestCaseModel>();
    
    private LoggerDetail myLogDetail = new LoggerDetail(typeof(ProjectDetailsViewModel));
    
    public ProjectDetailsViewModel(IStorageService storageService, ITestCaseRendererFactory rendererFactory, ISharedContext sharedContext)
    {
        myStorageService = storageService;
        myRendererFactory = rendererFactory;
        mySharedContext = sharedContext;
    }

    public IEnumerable<TestCaseStatus> TestCaseStatuses => Enum.GetValues<TestCaseStatus>();

    public void LoadTests(TestProjectModel project)
    {
        mySharedContext.OriginalTestCases.Clear();
        Tests.Clear();
        CurrentProject = project;
        foreach (var aModel in myStorageService.Load(project.DirectoryPath))
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
    }

    [RelayCommand]
    private void SaveTestCase()
    {
        using var aTrace = new LogScope(myLogDetail);
        var aSelectedTestRef = SelectedTestCase;
        if (aSelectedTestRef == null)
        {
            return;
        }

        if (!aSelectedTestRef.Validate())
        {
            aTrace.Warning($"Test input has errors, {string.Join("|", aSelectedTestRef.GetErrors().Select(x => x.ErrorMessage))}");
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
}