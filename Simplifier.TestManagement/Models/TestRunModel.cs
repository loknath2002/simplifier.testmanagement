// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
namespace Simplifier.TestManagement.Models;

public partial class TestStepResultModel : ObservableValidator
{
    [ObservableProperty]
    private int _StepNumber = -1;
    
    [ObservableProperty]
    private TestStepRunStatus _Status = TestStepRunStatus.Pending;
    
    [ObservableProperty]
    private string _Note = string.Empty;

    public TestStepResultModel(int theStepNumber, TestStepRunStatus theStatus)
    {
        this.StepNumber = theStepNumber;
        this.Status = theStatus;
    }
}

public partial class TestRun : ObservableValidator
{
    public TestRun(DateOnly theWhen)
    {
        this.When = theWhen;
    }
    
    [ObservableProperty]
    private DateOnly _When = DateOnly.MinValue;
    
    [ObservableProperty]
    private ObservableCollection<TestStepResultModel> _Results = [];
}