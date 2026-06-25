// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System.Collections.Generic;
namespace Simplifier.TestManagement.Models;

public enum TestStatus { NotStarted, Executing, Pass, Fail }

public class TestProjectModel
{
    
}

public class TestCaseModel
{
    public required TestProjectModel Project { get; set; }
    
    public required string Id { get; set; }
    
    public required string Name { get; set; }

    public string Precondition { get; set; } = string.Empty;

    public IList<string> Tags { get; set; } = [];

    public IList<TestStepModel> Steps { get; set; } = [];
}


public class TestStepModel
{
    public required int StepNumber { get; set; }
    
    public required string Procedure { get; set; }
    
    public required string ExpectedResult { get; set; }
    
    public required TestStatus Status { get; set; } = TestStatus.NotStarted;
    
    public string Comment { get; set; } = string.Empty;
}