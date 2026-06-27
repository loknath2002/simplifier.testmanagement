// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System;
using System.Collections.Generic;
using System.Globalization;
using Markdig;
using Markdig.Syntax;
using Markdown;
using Simplifier.TestManagement.Models;
using MarkdownDocument = Markdown.MarkdownDocument;
namespace Simplifier.TestManagement.Services;

public class MarkdownReadAndWriteImplV1 : ITestCaseRenderer
{
    private readonly MarkdownDocument myDocument = new MarkdownDocument();

    private readonly MarkdownPipeline myPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

    public TestCaseTarget Write(TestCaseModel theModelToSave)
    {
        const string aDateFormat = "dd-MMM-yyyy";
        myDocument.Append(new MarkdownHeader($"TC-{theModelToSave.Id}: {theModelToSave.Name}", 1));
        
        myDocument.Append(new MarkdownBlockquote(
            new MarkdownParagraph($"Author: {theModelToSave.MetaData.Author}"),
            new MarkdownParagraph($"Date: {theModelToSave.MetaData.Date.ToString(aDateFormat)}"),
            new MarkdownParagraph($"Tags: {string.Join(',', theModelToSave.MetaData.Tags)}"),
            new MarkdownParagraph($"Status: {string.Join(',', theModelToSave.MetaData.Status)}")
            ));
        
        myDocument.Append(new MarkdownHeader("Description", 2));
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
        var aDocument = Markdig.Markdown.Parse(theModelToRead.Content);
        foreach (var heading in aDocument.Descendants<QuoteBlock>())
        {
            Console.WriteLine($"Heading level {heading.ToString()}");
        }

        return null;
    }
}