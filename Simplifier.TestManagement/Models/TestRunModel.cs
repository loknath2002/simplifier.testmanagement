// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
namespace Simplifier.TestManagement.Models;

public partial class TestStepResultModel : ObservableValidator, IEquatable<TestStepResultModel>
{

    [ObservableProperty]
    private string _Note = string.Empty;

    [ObservableProperty]
    private TestStepStatus _Status = TestStepStatus.Pending;

    [ObservableProperty]
    private int _StepNumber = -1;

    public TestStepResultModel(int theStepNumber, TestStepStatus theStatus)
    {
        StepNumber = theStepNumber;
        Status = theStatus;
    }

    public override string ToString()
    {
        return $"{StepNumber} | {Status} | {Note}";
    }
    
    public bool Equals(TestStepResultModel? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Note == other.Note && Status == other.Status && StepNumber == other.StepNumber;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((TestStepResultModel)obj);
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Note, (int)Status, StepNumber);
    }
}

public partial class TestRun : ObservableValidator
{

    [ObservableProperty]
    private ObservableCollection<TestStepResultModel> _Results = [];

    [ObservableProperty]
    private DateOnly _When = DateOnly.FromDateTime(DateTime.Now);

    public TestRun(DateOnly theWhen)
    {
        When = theWhen;
    }
}