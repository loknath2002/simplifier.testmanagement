// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
namespace Simplifier.TestManagement.Models;

public partial class TestCaseMetadataModel : ObservableValidator
{

    [ObservableProperty]
    private string _Author = string.Empty;

    [ObservableProperty]
    private DateOnly _Date = DateOnly.FromDateTime(DateTime.Now);

    [ObservableProperty]
    private int _Revision;

    [ObservableProperty]
    private TestCaseStatus _Status = TestCaseStatus.Draft;

    [ObservableProperty]
    private ObservableCollection<string> _Tags = [];

    public string TagsString => string.Join(", ", Tags);
}