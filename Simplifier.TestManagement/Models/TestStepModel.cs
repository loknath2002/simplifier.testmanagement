// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
namespace Simplifier.TestManagement.Models;

#region Using statements

using System;
using CommunityToolkit.Mvvm.ComponentModel;

#endregion

public partial class TestStepModel : ObservableValidator, IEquatable<TestStepModel>
{
#region Fields

    [ObservableProperty]
    private string _ExpectedResult = string.Empty;

    [ObservableProperty]
    private int _Number = -1;

    [ObservableProperty]
    private string _Procedure = string.Empty;

    public TestStepModel()
    {
        
    }
    
    public TestStepModel(int theNumber, string theProcedure, string theResult)
    {
        this.Number = theNumber;
        this.Procedure = theProcedure;
        this.ExpectedResult = theResult;
    }

#endregion
#region IEquatable<TestStepModel> Members

    public bool Equals(TestStepModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ExpectedResult == other.ExpectedResult && Number == other.Number && Procedure == other.Procedure;
    }

#endregion
#region Members

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((TestStepModel)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ExpectedResult, Number, Procedure);
    }

    public override string ToString()
    {
        return $"{this.Number} | {this.Procedure} | {this.ExpectedResult}";
    }

#endregion
}