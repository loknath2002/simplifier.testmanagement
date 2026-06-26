// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System;
using System.IO;
using NUnit.Framework;
using Simplifier.TestManagement.Models;
using Simplifier.TestManagement.Services;
namespace Simplifier.TestManagement.Tests.Services;

[TestFixture]
[TestOf(typeof(MarkdownReadAndWriteImplV1))]
public class MarkdownReadAndWriteImplV1Test
{

    [Test]
    public void Verify_V1_TestCaseRenderer_Renders_Valid_Markdown()
    {
        TestCaseModel aModel = new TestCaseModel(new TestProjectModel("TestProject"), new TestCaseMetadataModel());
        aModel.Id = "1";
        aModel.Name = "Test Case 1";
        aModel.Description = "Test case description goes here.";
        aModel.Precondition = """
                              Test case preconditions go here.
                              * User is logged out.
                              * System is in a known state.
                              """;
        aModel.MetaData.Author = "TestCaseWriter";
        aModel.TestData = """
                          Test case data goes here.
                          * Username:something
                          * Password:something
                          """;
        aModel.MetaData.Date = new DateOnly(2020, 01, 01);
        aModel.MetaData.Tags.Add("Tag1");
        aModel.MetaData.Tags.Add("Tag2");
        aModel.Steps.Add(new TestStepModel(){ Number = 1, Procedure = "Navigate to the application login screen.", ExpectedResult = "Login form is displayed with Username and Password fields."});
        aModel.Steps.Add(new TestStepModel(){ Number = 2, Procedure = "Navigate to the application login screen.", ExpectedResult = "Login form is displayed with Username and Password fields."});
        aModel.Steps.Add(new TestStepModel(){ Number = 3, Procedure = "Enter valid `username` and `password` from Test Data.", ExpectedResult = "Characters are masked in the password field. Login button becomes active."});
        aModel.Steps.Add(new TestStepModel(){ Number = 4, Procedure = "Click the \"Login\" button.", ExpectedResult = "User is redirected to the dashboard. Session token is stored locally."});

        {
            var aRun = new TestRun(new DateOnly(2021, 01, 01));
            aRun.Results.Add(new TestStepResultModel(1, TestStepRunStatus.Pass));
            aRun.Results.Add(new TestStepResultModel(2, TestStepRunStatus.Fail) { Note = "2nd login fails" });
            aRun.Results.Add(new TestStepResultModel(3, TestStepRunStatus.Started));
            aRun.Results.Add(new TestStepResultModel(4, TestStepRunStatus.Pending));
            aModel.TestRuns.Add(aRun);
        }

        {
            var aRun = new TestRun(new DateOnly(2020, 01, 01));
            aRun.Results.Add(new TestStepResultModel(1, TestStepRunStatus.Pass));
            aRun.Results.Add(new TestStepResultModel(2, TestStepRunStatus.Pass));
            aRun.Results.Add(new TestStepResultModel(3, TestStepRunStatus.Pass));
            aRun.Results.Add(new TestStepResultModel(4, TestStepRunStatus.Pass));
            aModel.TestRuns.Add(aRun);
        }
        
        var aRenderer = new MarkdownReadAndWriteImplV1();
        string aFileContents = File.ReadAllText(Path.Combine("TestFiles", $"{nameof(Verify_V1_TestCaseRenderer_Renders_Valid_Markdown)}.md"));
        var aExpected = aRenderer.Write(aModel);
        Assert.That(aExpected.Content, Is.EqualTo(aFileContents));
        Assert.That(aExpected.FileName, Is.EqualTo("TestProject/TC_1_Test_Case_1.md"));
    }
}