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

using System.Collections.ObjectModel;

[TestFixture]
[TestOf(typeof(MarkdownReadAndWriteImplV1))]
public class MarkdownReadAndWriteImplV1Test
{
    [Test]
    public void Verify_V1_TestCaseRenderer_Reads_Valid_Markdown()
    {
        var aRenderer = new MarkdownReadAndWriteImplV1();
        string aFileContents = File.ReadAllText(Path.Combine("TestFiles", $"{nameof(Verify_V1_TestCaseRenderer_Reads_Valid_Markdown)}.md")).ToUnixEol();
        TestCaseModel aActual = aRenderer.Read(new TestCaseTarget(aFileContents, "RandomPath/TestProject/TC_1_Test_Case_1.md"));
        Assert.That(aActual.Id, Is.EqualTo(1));
        Assert.That(aActual.Name, Is.EqualTo("Test Case 1"));
        Assert.That(aActual.MetaData.Author, Is.EqualTo("##Description"));
        Assert.That(aActual.MetaData.Date, Is.EqualTo(new DateOnly(2020, 01, 01)));
        Assert.That(aActual.MetaData.Tags, Is.EqualTo(["Tag1", "Tag2"]));
        Assert.That(aActual.MetaData.Status, Is.EqualTo(TestCaseStatus.Draft));
        Assert.That(aActual.Description, Is.EqualTo("""
                                                    Test case description goes here.
                                                    SecondLine of description.
                                                    * User is logged out.
                                                    * System is in a known state.
                                                    """.ToUnixEol()));
        Assert.That(aActual.Precondition, Is.EqualTo("""
                                                    Test case preconditions go here.
                                                    * User is logged out.
                                                    * System is in a known state.
                                                    """.ToUnixEol()));
        
        
        Assert.That(aActual.TestData, Is.EqualTo("""
                                                     Test case data goes here.
                                                     * Username:something
                                                     * Password:something
                                                     """.ToUnixEol()));
        
        Assert.That(aActual.Steps, Is.EqualTo(new ObservableCollection<TestStepModel>()
        {
            new TestStepModel(1, "Navigate to the application login screen.", "Login form is displayed with Username and Password fields."),
            new TestStepModel(2, "Navigate to the application login screen.", "Login form is displayed with Username and Password fields."),
            new TestStepModel(3, "Enter valid `username` and `password` from Test Data.", "Characters are masked in the password field. Login button becomes active."),
            new TestStepModel(4, "Click the \"Login\" button.", "User is redirected to the dashboard. Session token is stored locally.")
        }));

        Assert.That(aActual.TestRuns[0].When, Is.EqualTo(new DateOnly(2021, 01, 01)));
        Assert.That(aActual.TestRuns[1].When, Is.EqualTo(new DateOnly(2020, 01, 01)));
        Assert.That(aActual.TestRuns[0].Results, Is.EqualTo(new ObservableCollection<TestStepResultModel>()
        {
            new TestStepResultModel(1, TestStepStatus.Pass),
            new TestStepResultModel(2, TestStepStatus.Fail) { Note = "2nd login fails" },
            new TestStepResultModel(3, TestStepStatus.Started),
            new TestStepResultModel(4, TestStepStatus.Pending),
        }));
        
        Assert.That(aActual.TestRuns[1].Results, Is.EqualTo(new ObservableCollection<TestStepResultModel>()
        {
            new TestStepResultModel(1, TestStepStatus.Pass),
            new TestStepResultModel(2, TestStepStatus.Pass),
            new TestStepResultModel(3, TestStepStatus.Pass),
            new TestStepResultModel(4, TestStepStatus.Pass),
        }));
    }
    
    [Test]
    public void Verify_V1_TestCaseRenderer_Writes_Valid_Markdown()
    {
        TestCaseModel aModel = new TestCaseModel(new TestProjectModel("TestProject"), new TestCaseMetadataModel());
        aModel.Id = 1;
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
            aRun.Results.Add(new TestStepResultModel(1, TestStepStatus.Pass));
            aRun.Results.Add(new TestStepResultModel(2, TestStepStatus.Fail) { Note = "2nd login fails" });
            aRun.Results.Add(new TestStepResultModel(3, TestStepStatus.Started));
            aRun.Results.Add(new TestStepResultModel(4, TestStepStatus.Pending));
            aModel.TestRuns.Add(aRun);
        }

        {
            var aRun = new TestRun(new DateOnly(2020, 01, 01));
            aRun.Results.Add(new TestStepResultModel(1, TestStepStatus.Pass));
            aRun.Results.Add(new TestStepResultModel(2, TestStepStatus.Pass));
            aRun.Results.Add(new TestStepResultModel(3, TestStepStatus.Pass));
            aRun.Results.Add(new TestStepResultModel(4, TestStepStatus.Pass));
            aModel.TestRuns.Add(aRun);
        }
        
        var aRenderer = new MarkdownReadAndWriteImplV1();
        string aFileContents = File.ReadAllText(Path.Combine("TestFiles", $"{nameof(Verify_V1_TestCaseRenderer_Writes_Valid_Markdown)}.md")).ToUnixEol();
        var aExpected = aRenderer.Write(aModel);
        Assert.That(aExpected.Content, Is.EqualTo(aFileContents));
        Assert.That(aExpected.FileName, Is.EqualTo("TestProject/TC_1_Test_Case_1.md"));
    }
}