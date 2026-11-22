// /*
// * Copyright (c) 2026 Loknath
// */
using System;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
namespace Simplifier.TestManagement.Models;

#region Using statements



#endregion

public partial class TestStepModel : ObservableValidator, IEquatable<TestStepModel>
{

    #region IEquatable<TestStepModel> Members

    public bool Equals(TestStepModel? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ExpectedResult == other.ExpectedResult && Number == other.Number && Procedure == other.Procedure;
    }

    #endregion

    #region Fields

    [Required]
    [ObservableProperty]
    private string _ExpectedResult = string.Empty;

    [Required]
    [ObservableProperty]
    private int _Number = -1;

    [Required]
    [ObservableProperty]
    private string _Procedure = string.Empty;

    public TestStepModel()
    {

    }

    internal bool Validate()
    {
        ValidateAllProperties();
        return !HasErrors;
    }

    public TestStepModel(int theNumber, string theProcedure, string theResult)
    {
        Number = theNumber;
        Procedure = theProcedure;
        ExpectedResult = theResult;
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
        return $"{Number} | {Procedure} | {ExpectedResult}";
    }

    #endregion

}