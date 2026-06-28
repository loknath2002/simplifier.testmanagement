// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
namespace Simplifier.TestManagement.Models;

public partial class TestCaseModel : ObservableValidator
{
    public string SchemaVersion => ModelVersionConstants.SSTM1;
    
    [ObservableProperty]
    private TestCaseMetadataModel _MetaData;
    
    [ObservableProperty]
    private string _Description = string.Empty;

    [Required]
    [ObservableProperty]
    private int _Id = -1;

    [Required]
    [ObservableProperty]
    private string _Name = string.Empty;

    [ObservableProperty]
    private string _Precondition = string.Empty;

    [Required]
    [ObservableProperty]
    private TestProjectModel _Project;

    [ObservableProperty]
    private ObservableCollection<TestStepModel> _Steps = [];

    [ObservableProperty]
    private ObservableCollection<TestRun> _TestRuns = [];
    
    [ObservableProperty]
    private string _TestData = string.Empty;

    public Guid UniqueId { get; init; } = Guid.NewGuid();

    public TestCaseModel(TestProjectModel theTestProject, TestCaseMetadataModel metaData)
    {
        this.MetaData = metaData;
        this.Project = theTestProject;
    }

    public string StoragePath()
    {
        string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        var r = new Regex($"[{Regex.Escape(regexSearch)}]");
        var aSanatized = r.Replace(this.Name, "");
        return PathExtensions.CombineWithForwardSlash(this.Project.DirectoryPath, $"TC_{this.Id}_{aSanatized}.md".Replace("-", "_").Replace(" ", "_"));
    }
    
    public bool Validate()
    {
        this.ValidateAllProperties();
        return !this.HasErrors;
    }
}