// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using CommunityToolkit.Mvvm.ComponentModel;
namespace Simplifier.TestManagement.Models;

public partial class TestStepModel : ObservableValidator
{
    
    [ObservableProperty]
    private string _ExpectedResult = string.Empty;

    [ObservableProperty]
    private int _Number = -1;

    [ObservableProperty]
    private string _Procedure = string.Empty;

}