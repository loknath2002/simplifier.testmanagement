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
        TestCaseModel aModel = new TestCaseModel(TestProjectModel.FromFullPath("TestProject"), new TestCaseMetadataModel());
        aModel.Id = 1;
        aModel.Name = "Test Case 1";
        aModel.Description = """
                             Test case description goes here.
                             ## Description
                             """;
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

    [Test]
    public void Verify_TestStep_With_Newline_Encoding_Decoding()
    {
        var aRenderer = new MarkdownReadAndWriteImplV1();
        
        // Setup a model with a newline in a test step
        var aModel = new TestCaseModel(TestProjectModel.FromFullPath("TestProject"), new TestCaseMetadataModel());
        aModel.Id = 1;
        aModel.Name = "Test Case Newline";
        aModel.Description = "Desc";
        aModel.Precondition = "Pre";
        aModel.TestData = "Data";
        aModel.MetaData.Author = "Author";
        aModel.MetaData.Date = new DateOnly(2020, 01, 01);
        
        string procedureWithNewline = "Step 1\nLine 2";
        string expectedResultWithNewline = "Result 1\nLine 2";
        aModel.Steps.Add(new TestStepModel(1, procedureWithNewline, expectedResultWithNewline));
        
        // Write it
        var aTarget = aRenderer.Write(aModel);
        
        // Check if markdown contains <br> instead of newline in the table
        // The markdown table structure should have <br>
        Assert.That(aTarget.Content, Does.Contain("Step 1<br>Line 2"));
        Assert.That(aTarget.Content, Does.Contain("Result 1<br>Line 2"));
        // Should not contain raw newline inside the table row
        
        // Read it back
        var aReadModel = aRenderer.Read(aTarget);
        
        Assert.That(aReadModel.Steps[0].Procedure, Is.EqualTo(procedureWithNewline));
        Assert.That(aReadModel.Steps[0].ExpectedResult, Is.EqualTo(expectedResultWithNewline));
    }

    [Test]
    public void Verify_TestStep_With_CRLF_Newline_Encoding_Decoding()
    {
        var aRenderer = new MarkdownReadAndWriteImplV1();
        
        // Setup a model with a CRLF newline in a test step
        var aModel = new TestCaseModel(TestProjectModel.FromFullPath("TestProject"), new TestCaseMetadataModel());
        aModel.Id = 1;
        aModel.Name = "Test Case CRLF Newline";
        aModel.Description = "Desc";
        aModel.Precondition = "Pre";
        aModel.TestData = "Data";
        aModel.MetaData.Author = "Author";
        aModel.MetaData.Date = new DateOnly(2020, 01, 01);
        
        string procedureWithCRLF = "Step 1\r\nLine 2";
        string expectedResultWithCRLF = "Result 1\r\nLine 2";
        aModel.Steps.Add(new TestStepModel(1, procedureWithCRLF, expectedResultWithCRLF));
        
        // Write it
        var aTarget = aRenderer.Write(aModel);
        
        // Check if markdown contains <br> instead of newline in the table
        // The markdown table structure should have <br>
        Assert.That(aTarget.Content, Does.Contain("Step 1<br>Line 2"));
        Assert.That(aTarget.Content, Does.Contain("Result 1<br>Line 2"));
        // Should not contain raw newline inside the table row
        
        // Read it back
        var aReadModel = aRenderer.Read(aTarget);
        
        // Newlines are normalized to \n during reading
        Assert.That(aReadModel.Steps[0].Procedure, Is.EqualTo(procedureWithCRLF.Replace("\r\n", "\n")));
        Assert.That(aReadModel.Steps[0].ExpectedResult, Is.EqualTo(expectedResultWithCRLF.Replace("\r\n", "\n")));
    }

    [Test]
    public void Verify_RunHistory_Note_With_Newline_Encoding_Decoding()
    {
        var aRenderer = new MarkdownReadAndWriteImplV1();
        
        // Setup a model with a newline in a run history note
        var aModel = new TestCaseModel(TestProjectModel.FromFullPath("TestProject"), new TestCaseMetadataModel());
        aModel.Id = 1;
        aModel.Name = "Test Case Run History Newline";
        aModel.Description = "Desc";
        aModel.Precondition = "Pre";
        aModel.TestData = "Data";
        aModel.MetaData.Author = "Author";
        aModel.MetaData.Date = new DateOnly(2020, 01, 01);
        
        var aRun = new TestRun(new DateOnly(2021, 01, 01));
        string noteWithNewline = "Note 1\nLine 2";
        aRun.Results.Add(new TestStepResultModel(1, TestStepStatus.Pass) { Note = noteWithNewline });
        aModel.TestRuns.Add(aRun);
        
        // Write it
        var aTarget = aRenderer.Write(aModel);
        
        // Check if markdown contains <br> instead of newline in the table
        Assert.That(aTarget.Content, Does.Contain("Note 1<br>Line 2"));
        // Should not contain raw newline inside the table row
        
        // Read it back
        var aReadModel = aRenderer.Read(aTarget);
        
        Assert.That(aReadModel.TestRuns[0].Results[0].Note, Is.EqualTo(noteWithNewline));
    }
}