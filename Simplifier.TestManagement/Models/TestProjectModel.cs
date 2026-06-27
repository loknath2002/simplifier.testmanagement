// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System.ComponentModel.DataAnnotations;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
namespace Simplifier.TestManagement.Models;

using System;

public partial class TestProjectModel : ObservableValidator
{
    public TestProjectModel(string theDirectoryPath)
    {
        this.DirectoryPath = theDirectoryPath;
        this.Name = Path.GetDirectoryName(theDirectoryPath) ?? string.Empty;
    }
    
    [Required(ErrorMessage = "Project path is required")]
    [NotifyDataErrorInfo]
    [ObservableProperty]
    private string _DirectoryPath;

    [Required(ErrorMessage = "Project name is required")]
    [NotifyDataErrorInfo]
    [ObservableProperty]
    private string _Name;

    internal bool Validate()
    {
        this.ValidateAllProperties();
        return !this.HasErrors;
    }
}