// /*
// * Copyright (c) 2026 Loknath
// */
using System;
using System.Collections.Generic;
using System.IO;
using Simplifier.TestManagement.Models;

namespace Simplifier.TestManagement.Services;

public class StorageServiceImpl : IStorageService
{
    private readonly ITestCaseRendererFactory theRendererFactory;

    public StorageServiceImpl(ITestCaseRendererFactory theRendererFactory)
    {
        this.theRendererFactory = theRendererFactory;
    }

    public bool Save(TestCaseModel theOriginalPath, TestCaseModel theNewPath, string theContent)
    {
        return Save(theOriginalPath.StoragePath(), theNewPath.StoragePath(), theContent);
    }
    
    public bool Save(string theOriginalPath, string theNewPath, string theContent)
    {
        string aTempPath = Path.Combine(Paths.TempPath, Guid.NewGuid().ToString());
        File.WriteAllText(aTempPath, theContent);
        if (theOriginalPath == theNewPath)
        {
            File.Move(aTempPath, theOriginalPath, true);
        }
        else
        {
            File.Move(aTempPath, theOriginalPath, true);
            File.Move(theOriginalPath, theNewPath, true);
        }
        
        return true;
    }

    public IEnumerable<TestCaseModel> Load(string theProjectDirectory)
    {
        var aFiles = Directory.GetFiles(theProjectDirectory, "*.md");
        foreach (var aFile in aFiles)
        {
            string aContent = File.ReadAllText(aFile);
            var aTarget = new TestCaseTarget(aContent, aFile);
            var aRenderer = theRendererFactory.Create(aTarget);
            yield return aRenderer.Read(aTarget);
        }
    }

    public void Delete(string theStoragePath)
    {
        if (File.Exists(theStoragePath))
        {
            File.Delete(theStoragePath);
        }
    }
}