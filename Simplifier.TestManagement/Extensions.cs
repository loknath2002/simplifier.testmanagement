// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System.IO;
using System.Collections.ObjectModel;
using Simplifier.TestManagement.Models;

namespace Simplifier.TestManagement;

public static class NewLineExtensions
{
    public static string EOL = "\n";
    public static char EOLC = '\n';
    
    public static string ToUnixEol(this string theLine)
    {
        return theLine.Replace("\r\n", EOL).Replace('\r', EOLC);
    }

    public static string ToMarkdownTableEol(this string theLine)
    {
        return theLine.ToUnixEol().Replace("\n", "<br>");
    }

    public static string SanitizeMarkdownColumn(this string theLine)
    {
        return theLine.Replace("<br>", EOL);
    }
}

public static class PathExtensions
{
    public static string CombineWithForwardSlash(params string[] paths)
    {
        return Path.Combine(paths).Replace('\\', '/');
    }
}

public static class TestCaseExtensions
{
    public static TestCaseModel DeepCopy(this TestCaseModel original)
    {
        var copy = new TestCaseModel(original.Project, original.MetaData)
        {
            Description = original.Description,
            Id = original.Id,
            Name = original.Name,
            Precondition = original.Precondition,
            TestData = original.TestData,
            UniqueId = original.UniqueId
        };

        foreach (var step in original.Steps)
        {
            copy.Steps.Add(new TestStepModel(step.Number, step.Procedure, step.ExpectedResult));
        }

        foreach (var run in original.TestRuns)
        {
            var runCopy = new TestRun(run.When);
            foreach (var result in run.Results)
            {
                runCopy.Results.Add(new TestStepResultModel(result.StepNumber, result.Status)
                {
                    Note = result.Note,
                    Procedure = result.Procedure,
                    ExpectedResult = result.ExpectedResult
                });
            }
            copy.TestRuns.Add(runCopy);
        }
        return copy;
    }
}