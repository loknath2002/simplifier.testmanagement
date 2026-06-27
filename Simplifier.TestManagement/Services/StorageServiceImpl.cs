// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System;
using System.Collections.Generic;
using System.IO;
using Simplifier.TestManagement.Models;

namespace Simplifier.TestManagement.Services;

public class StorageServiceImpl : IStorageService
{
    private readonly ITestCaseRendererFactory _rendererFactory;

    public StorageServiceImpl(ITestCaseRendererFactory rendererFactory)
    {
        _rendererFactory = rendererFactory;
    }

    public bool Save(string thePath, string theContent)
    {
        string aTempPath = Path.Combine(Paths.TempPath, Guid.NewGuid().ToString());
        File.WriteAllText(aTempPath, theContent);
        File.Move(aTempPath, thePath, true);
        return true;
    }

    public IEnumerable<TestCaseModel> Load(string theProjectDirectory)
    {
        var aFiles = Directory.GetFiles(theProjectDirectory, "*.md");
        foreach (var aFile in aFiles)
        {
            string aContent = File.ReadAllText(aFile);
            var aTarget = new TestCaseTarget(aContent, aFile);
            var aRenderer = _rendererFactory.Create(aTarget);
            yield return aRenderer.Read(aTarget);
        }
    }
}