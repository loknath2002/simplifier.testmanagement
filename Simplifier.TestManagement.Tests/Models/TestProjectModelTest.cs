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
        var aTestModel = new TestProjectModel(null!, null!);
        Assert.That(aTestModel.Validate(), Is.False);
        Assert.That(aTestModel.GetErrors().Count(), Is.EqualTo(2));
        Assert.That(aTestModel.GetErrors().Count(x=> x.ErrorMessage == "Project name is required"), Is.EqualTo(1));
        Assert.That(aTestModel.GetErrors().Count(x=> x.ErrorMessage == "Project path is required"), Is.EqualTo(1));
    }

    [Test]
    public void TestProjectModelWithInvalidCharacters()
    {
        var aTestModel = new TestProjectModel("C:\\Invalid|Path", "Invalid?Name");
        Assert.That(aTestModel.Validate(), Is.False);
        Assert.That(aTestModel.GetErrors().Count(x => x.ErrorMessage == "Project path contains invalid characters"), Is.EqualTo(1));
        Assert.That(aTestModel.GetErrors().Count(x => x.ErrorMessage == "Project name contains invalid characters"), Is.EqualTo(1));
    }

    [Test]
    public void FromFullPath_WithDots_ReturnsCorrectName()
    {
        var model = TestProjectModel.FromFullPath("C:\\Projects\\MyProject.v1");
        Assert.That(model.Name, Is.EqualTo("MyProject.v1"));
        Assert.That(model.DirectoryPath, Is.EqualTo("C:\\Projects"));
    }

    [Test]
    public void FromFullPath_WithTrailingSlash_ReturnsCorrectName()
    {
        var model = TestProjectModel.FromFullPath("C:\\Projects\\MyProject\\");
        Assert.That(model.Name, Is.EqualTo("MyProject"));
        Assert.That(model.DirectoryPath, Is.EqualTo("C:\\Projects"));
    }
}