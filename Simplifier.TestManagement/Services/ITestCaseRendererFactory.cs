// /*
// * Copyright (c) 2026 Loknath
// */
using Simplifier.TestManagement.Models;
namespace Simplifier.TestManagement.Services;

public interface ITestCaseRendererFactory
{
    ITestCaseRenderer Create(TestCaseTarget theTarget);
    ITestCaseRenderer Create(TestCaseModel theModel);
}