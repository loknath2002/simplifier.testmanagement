// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using Simplifier.TestManagement.Models;
namespace Simplifier.TestManagement.Services;

public interface ITestCaseRenderer
{
    TestCaseTarget Write(TestCaseModel theModelToSave);
    TestCaseModel Read(TestCaseTarget theModelToRead);
}