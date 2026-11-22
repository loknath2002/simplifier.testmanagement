// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System.ComponentModel.DataAnnotations;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using Simplifier.TestManagement.Validators;
namespace Simplifier.TestManagement.Models;

using System;

public partial class TestProjectModel : ObservableValidator
{

    public TestProjectModel()
    {
        
    }
    
    public TestProjectModel(string theDirectoryPath, string theProjectName)
    {
        this.DirectoryPath = theDirectoryPath;
        this.Name = theProjectName;
    }

    public static TestProjectModel FromFullPath(string theDirectoryAndProjectPath)
    {
        var cleanedPath = theDirectoryAndProjectPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return new TestProjectModel(Path.GetDirectoryName(cleanedPath), Path.GetFileName(cleanedPath));
    }
    
    [Required(ErrorMessage = "Project path is required")]
    [NotifyDataErrorInfo]
    [ObservableProperty]
    [InvalidCharacters(isPath: true, ErrorMessage = "Project path contains invalid characters")]
    private string _DirectoryPath;

    [Required(ErrorMessage = "Project name is required")]
    [NotifyDataErrorInfo]
    [ObservableProperty]
    [InvalidCharacters(isPath: false, ErrorMessage = "Project name contains invalid characters")]
    private string _Name;

    public string ProjectPath => Path.Combine(this.DirectoryPath, this.Name);
    
    internal bool Validate()
    {
        this.ValidateAllProperties();
        return !this.HasErrors;
    }
}