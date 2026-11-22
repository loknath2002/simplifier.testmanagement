using NUnit.Framework;
using Simplifier.TestManagement.Models;
using Simplifier.TestManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Simplifier.TestManagement.Services;
using Simplifier.TestManagement.Validators;

namespace Simplifier.TestManagement.Tests.Models;

[TestFixture]
public class TestCaseModelTest
{
    [Test]
    public void TestCaseModel_HasUniqueId()
    {
        var project = TestProjectModel.FromFullPath("TestPath");
        var metaData = new TestCaseMetadataModel();
        var tc1 = new TestCaseModel(project, metaData);
        var tc2 = new TestCaseModel(project, metaData);

        Assert.That(tc1.UniqueId, Is.Not.EqualTo(Guid.Empty));
        Assert.That(tc2.UniqueId, Is.Not.EqualTo(Guid.Empty));
        Assert.That(tc1.UniqueId, Is.Not.EqualTo(tc2.UniqueId));
    }

    [Test]
    public void TestCaseModel_DeepCopy_CreatesIndependentCopy()
    {
        var project = TestProjectModel.FromFullPath("TestPath");
        var metaData = new TestCaseMetadataModel();
        var tc = new TestCaseModel(project, metaData)
        {
            Description = "Desc",
            Id = 1,
            Name = "Name",
            Precondition = "Pre",
            TestData = "Data"
        };
        tc.Steps.Add(new TestStepModel(1, "Proc", "Result"));
        var run = new TestRun(DateTime.Now);
        run.Results.Add(new TestStepResultModel(1, TestStepStatus.Pass));
        tc.TestRuns.Add(run);

        var copy = tc.DeepCopy();

        Assert.That(copy.UniqueId, Is.EqualTo(tc.UniqueId));
        Assert.That(copy.Description, Is.EqualTo(tc.Description));
        Assert.That(copy.Id, Is.EqualTo(tc.Id));
        Assert.That(copy.Name, Is.EqualTo(tc.Name));
        Assert.That(copy.Precondition, Is.EqualTo(tc.Precondition));
        Assert.That(copy.TestData, Is.EqualTo(tc.TestData));
        
        Assert.That(copy.Steps.Count, Is.EqualTo(1));
        Assert.That(copy.Steps[0].Procedure, Is.EqualTo(tc.Steps[0].Procedure));
        Assert.That(copy.Steps[0], Is.Not.SameAs(tc.Steps[0]));
        
        Assert.That(copy.TestRuns.Count, Is.EqualTo(1));
        Assert.That(copy.TestRuns[0].Results.Count, Is.EqualTo(1));
        Assert.That(copy.TestRuns[0], Is.Not.SameAs(tc.TestRuns[0]));
    }
    [Test]
    public void TestCaseModel_DuplicateId_ValidationFails()
    {
        var project = TestProjectModel.FromFullPath("TestPath");
        var metaData = new TestCaseMetadataModel();
        var tc1 = new TestCaseModel(project, metaData) { Id = 1 };
        var tc2 = new TestCaseModel(project, metaData) { Id = 1 };

        var sharedContext = new SharedContext();
        sharedContext.OriginalTestCases.Add(tc1.UniqueId, tc1);
        TestModelDuplicateHandler.SharedContext = sharedContext;

        Assert.That(tc2.Validate(), Is.False);
        Assert.That(tc2.HasErrors, Is.True);
    }
}
