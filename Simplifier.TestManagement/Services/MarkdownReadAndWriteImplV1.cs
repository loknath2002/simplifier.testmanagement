// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Markdown;
using Simplifier.TestManagement.Models;
using MarkdownDocument = Markdown.MarkdownDocument;
namespace Simplifier.TestManagement.Services;

using System.Text;

public class MarkdownReadAndWriteImplV1 : ITestCaseRenderer
{
    private const string aDateFormat = "dd-MMM-yyyy";
    private const string DescriptionTag = "Description";
    private const string TCPrefix = "TC-";
    private const string AuthorTag = "Author";
    private const string DateTag = "Date";
    private const string TagsTag = "Tags";
    private const string StatusTag = "Status";
    
    private readonly MarkdownDocument myDocument = new MarkdownDocument();


    public TestCaseTarget Write(TestCaseModel theModelToSave)
    {
        myDocument.Append(new MarkdownHeader($"{TCPrefix}{theModelToSave.Id}: {theModelToSave.Name}", 1));
        
        myDocument.Append(new MarkdownBlockquote(
            new MarkdownParagraph($"{AuthorTag}: {theModelToSave.MetaData.Author}"),
            new MarkdownParagraph($"{DateTag}: {theModelToSave.MetaData.Date.ToString(aDateFormat)}"),
            new MarkdownParagraph($"{TagsTag}: {string.Join(',', theModelToSave.MetaData.Tags)}"),
            new MarkdownParagraph($"{StatusTag}: {string.Join(',', theModelToSave.MetaData.Status)}")
            ));

        myDocument.Append(new MarkdownHeader(DescriptionTag, 2));
        myDocument.AppendParagraph(theModelToSave.Description);
        
        myDocument.Append(new MarkdownHeader("Preconditions", 2));
        myDocument.AppendParagraph(theModelToSave.Precondition);
        
        myDocument.Append(new MarkdownHeader("Dataset", 2));
        myDocument.AppendParagraph(theModelToSave.TestData);

        {
            myDocument.Append(new MarkdownHeader("Steps & Expected Results", 2));
            List<MarkdownTableRow> aRows = [];
            foreach (TestStepModel aStep in theModelToSave.Steps)
            {
                aRows.Add(new MarkdownTableRow(aStep.Number.ToString(CultureInfo.InvariantCulture), aStep.Procedure, aStep.ExpectedResult));
            }

            myDocument.AppendTable(new MarkdownTableHeader(
                    new MarkdownTableHeaderCell("Step"),
                    new MarkdownTableHeaderCell("Procedure"),
                    new MarkdownTableHeaderCell("Expected Result")),
                aRows);
        }

        {
            myDocument.Append(new MarkdownHeader("Run History", 2));
            foreach (TestRun aRun in theModelToSave.TestRuns)
            {
                myDocument.Append(new MarkdownHeader($"{aRun.When.ToString(aDateFormat)}", 3));
                List<MarkdownTableRow> aResultTableRows = [];
                foreach (TestStepResultModel aResult in aRun.Results)
                {
                    aResultTableRows.Add(new MarkdownTableRow(aResult.StepNumber.ToString(CultureInfo.InvariantCulture), aResult.Status.ToString(), aResult.Note));
                }

                myDocument.AppendTable(new MarkdownTableHeader(
                        new MarkdownTableHeaderCell("Step"),
                        new MarkdownTableHeaderCell("Status"),
                        new MarkdownTableHeaderCell("Notes")),
                    aResultTableRows);
            }
        }

        {
            myDocument.Append(new MarkdownHeader("Internal Program Data [DO NOT CHANGE OR REMOVE]", 4));
            myDocument.Append(new MarkdownBlockquote(
                new MarkdownParagraph($"Program Used: Super Simple Test Management (SSTM)"),
                new MarkdownParagraph($"SchemaVersion: 1")
            ));
        }
        
        return new TestCaseTarget(myDocument.ToPrettyString().Replace("\r\n", "\n"), theModelToSave.StoragePath());
    }
    
    public TestCaseModel Read(TestCaseTarget theModelToRead)
    {
        TestProjectModel aTestProjectModel = new TestProjectModel(Path.GetDirectoryName(theModelToRead.FileName));
        TestCaseModel aModel = new TestCaseModel(aTestProjectModel, new TestCaseMetadataModel());
        string []aLines = theModelToRead.Content.Replace("\r\n", "\n").Split("\n");
        string aTitleAndMetaRegion = GetRegion(aLines, $"# {TCPrefix}", $"## {DescriptionTag}", ReadExceptionTypes.MissingTestTitleAndMetaRegion);
        ExtractTestIDAndTitle(aTitleAndMetaRegion, aModel);
        ExtractMetaData(aTitleAndMetaRegion, aModel);
        
        
        return aModel;
    }

    private void ExtractMetaData(string theTitleAndMetaRegion, TestCaseModel theModel)
    {
        string[] aLines = theTitleAndMetaRegion.Split('\n');
        string []aPrefixes = [AuthorTag, DateTag, TagsTag, StatusTag];
        string []aSuffix = [">", ">", ">", ""];
        ReadExceptionTypes []aExceptionTypes = [ReadExceptionTypes.CorruptTestAuthor, ReadExceptionTypes.CorruptTestDate, ReadExceptionTypes.CorruptTestTags, ReadExceptionTypes.CorruptTestStatus];
        for (int i = 0; i < aPrefixes.Length; i++)
        {
            string aMetaParts = GetRegion(aLines, $"> {aPrefixes[i]}", aSuffix[i], ReadExceptionTypes.CorruptTestMetadata);
            string[] aParts = aMetaParts.Split(':', 2);
            if (aParts.Length != 2)
            {
                throw new TestCaseReadException(aExceptionTypes[i]);
            }

            if (i == 0)
            {
                theModel.MetaData.Author = aParts[1].Trim();
            }
            
            if (i == 1)
            {
                if(!DateOnly.TryParseExact(aParts[1].Trim(), aDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly aDate))
                {
                    throw new TestCaseReadException(ReadExceptionTypes.CorruptTestDate);
                }

                theModel.MetaData.Date = aDate;
            }
            
            if (i == 2)
            {
                foreach (var aTag in aParts[1].Trim().Split(',', StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries))
                {
                    theModel.MetaData.Tags.Add(aTag);
                }
            }
            
            if (i == 3)
            {
                if (!Enum.TryParse(aParts[1].Trim(), out TestCaseStatus aStatus))
                {
                    throw new TestCaseReadException(ReadExceptionTypes.CorruptTestStatus);
                }
                
                theModel.MetaData.Status = aStatus;
            }
        }
    }

    private static void ExtractTestIDAndTitle(string aTitleAndMetaRegion, TestCaseModel aModel)
    {
        string []aLine = aTitleAndMetaRegion.Split('\n');
        if (aLine.Length < 1)
        {
            throw new TestCaseReadException(ReadExceptionTypes.MissingTestTitle);
        }

        string[] aParts = aLine[0].Split(':', 2);
        if (aParts.Length != 2)
        {
            throw new TestCaseReadException(ReadExceptionTypes.CorruptTestTitle);
        }
            
        aModel.Id = aParts[0].Replace("#", string.Empty).Replace(TCPrefix, string.Empty).Trim();
        aModel.Name = aParts[1].Trim();
    }

    private string GetRegion(string[] theLines, string thePrefix1, string thePrefix2, ReadExceptionTypes theExceptionTypes)
    {
        int aStartIndex = -1;
        int aEndIndex = -1;
        for (int i = 0; i < theLines.Length; i++)
        {
            if (theLines[i].StartsWith(thePrefix1, StringComparison.InvariantCulture) && aStartIndex == -1)
            {
                aStartIndex = i;
                break;
            }
        }

        if (aStartIndex == -1)
        {
            throw new TestCaseReadException(theExceptionTypes);
        }
        
        for (int i = aStartIndex + 1; i < theLines.Length; i++)
        {
            if (theLines[i].StartsWith(thePrefix2, StringComparison.InvariantCulture))
            {
                aEndIndex = i;
                break;
            }
        }
        
        if(aEndIndex == -1 || aEndIndex <= aStartIndex)
        {
            throw new TestCaseReadException(theExceptionTypes);
        }

        StringBuilder aBuilder = new StringBuilder();
        for (int i = aStartIndex; i < aEndIndex; i++)
        {
            aBuilder.Append($"{theLines[i]}\n");
        }

        return aBuilder.ToString();
    }
}