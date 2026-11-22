// /*
// * Copyright (c) 2026 Loknath
// */
using System.Collections;
using System.Collections.Generic;
using Simplifier.TestManagement.Models;
namespace Simplifier.TestManagement.Services;

public interface IStorageService
{
    bool Save(TestCaseModel theOriginalPath, TestCaseModel theNewPath, string theContent);
    IEnumerable<TestCaseModel> Load(string theProjectDirectory);
    void Delete(string theStoragePath);
}