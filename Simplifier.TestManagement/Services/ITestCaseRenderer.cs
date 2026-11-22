// /*
// * Copyright (c) 2026 Loknath
// */
using Simplifier.TestManagement.Models;
namespace Simplifier.TestManagement.Services;

public interface ITestCaseRenderer
{
    TestCaseTarget Write(TestCaseModel theModelToSave);
    TestCaseModel Read(TestCaseTarget theModelToRead);
}