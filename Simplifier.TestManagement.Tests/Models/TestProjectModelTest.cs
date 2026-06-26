// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System;
using System.Linq;
using NUnit.Framework;
using Simplifier.TestManagement.Models;
namespace Simplifier.TestManagement.Tests.Models;

[TestFixture]
[TestOf(typeof(TestProjectModel))]
public class TestProjectModelTest
{
    [Test]
    public void CreateTestProjectModel()
    {
        var aTestModel = new TestProjectModel(null!);
        Assert.That(aTestModel.Validate(), Is.False);
        Assert.That(aTestModel.GetErrors().Count(), Is.EqualTo(2));
        Assert.That(aTestModel.GetErrors().Count(x=> x.ErrorMessage == "Project name is required"), Is.EqualTo(1));
        Assert.That(aTestModel.GetErrors().Count(x=> x.ErrorMessage == "Project path is required"), Is.EqualTo(1));
    }
}