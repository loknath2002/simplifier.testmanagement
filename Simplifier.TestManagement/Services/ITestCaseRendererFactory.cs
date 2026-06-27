// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using Simplifier.TestManagement.Models;
namespace Simplifier.TestManagement.Services;

public interface ITestCaseRendererFactory
{
    ITestCaseRenderer Create(TestCaseTarget theTarget);
    ITestCaseRenderer Create(TestCaseModel theModel);
}