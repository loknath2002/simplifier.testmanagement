// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System.Diagnostics.CodeAnalysis;
namespace Simplifier.TestManagement.Services;

public class TestCaseTarget
{
    public TestCaseTarget(string theContent, string theFileName)
    {
        Content = theContent;
        FileName = theFileName;
    }
    
    public string Content { get; }
    
    public string FileName { get; }
}