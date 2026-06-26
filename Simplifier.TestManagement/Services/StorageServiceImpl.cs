// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System;
using System.IO;
using Simplifier.TestManagement.Models;
namespace Simplifier.TestManagement.Services;

public class StorageServiceImpl : IStorageService
{
    public bool Save(string thePath, string theContent)
    {
        string aTempPath = Path.Combine(Paths.TempPath, Guid.NewGuid().ToString());
        File.WriteAllText(aTempPath, theContent);
        File.Move(aTempPath, thePath, true);
        return true;
    }
}