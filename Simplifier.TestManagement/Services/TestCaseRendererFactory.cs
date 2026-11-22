using System;
using System.IO;
using System.Linq;
using Simplifier.TestManagement.Models;

namespace Simplifier.TestManagement.Services;

public class TestCaseRendererFactory : ITestCaseRendererFactory
{
    public ITestCaseRenderer Create(TestCaseTarget theTarget)
    {
        var lines = theTarget.Content.Split(NewLineExtensions.EOLC);
        var lastLine = lines.LastOrDefault(l => !string.IsNullOrWhiteSpace(l))?.Trim();

        if (lastLine == $"> SchemaVersion: {ModelVersionConstants.SSTM1}")
        {
            return new MarkdownReadAndWriteImplV1();
        }
        
        throw new NotSupportedException("Unsupported schema version or version not on last line");
    }

    public ITestCaseRenderer Create(TestCaseModel theModel)
    {
        if (theModel.SchemaVersion == ModelVersionConstants.SSTM1)
        {
            return new MarkdownReadAndWriteImplV1();
        }
        
        throw new NotSupportedException("Unsupported schema version");
    }
}
