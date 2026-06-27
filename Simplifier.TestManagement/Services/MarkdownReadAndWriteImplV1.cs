// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Markdown;
using Simplifier.TestManagement.Models;
namespace Simplifier.TestManagement.Services;

#region Using statements



#endregion

public class MarkdownReadAndWriteImplV1 : ITestCaseRenderer
{

    #region Fields

    private const string DateFormat = "dd-MMM-yyyy";
    private const string RevisionTag = "Revision";
    private const string AuthorTag = "Author";
    private const string DatasetTag = "Dataset";
    private const string DateTag = "Date";
    private const string DescriptionTag = "Description";
    private const string PreconditionsTag = "Preconditions";
    private const string RunHistoryTag = "Run History";
    private const string InternalProgramDataTag = "Internal Program Data [DO NOT CHANGE OR REMOVE]";
    private const string StatusTag = "Status";
    private const string StepsAndExpectedResultsTag = "Steps & Expected Results";
    private const string TagsTag = "Tags";
    private const string TCPrefix = "TC-";
    private const string RunPrefix = "Run-";

    private readonly MarkdownDocument myDocument = new MarkdownDocument();

    #endregion

    #region ITestCaseRenderer Members

    public TestCaseModel Read(TestCaseTarget theModelToRead)
    {
        var aTestProjectModel = new TestProjectModel(Path.GetDirectoryName(theModelToRead.FileName));
        var aModel = new TestCaseModel(aTestProjectModel, new TestCaseMetadataModel());
        string[] aLines = theModelToRead.Content.ToUnixEol().Split(NewLineExtensions.EOLC);
        string aDescHeader = $"## {DescriptionTag}";
        string aPreHeader = $"## {PreconditionsTag}";
        string aDataHeader = $"## {DatasetTag}";
        string aStepsAndResultsHeader = $"## {StepsAndExpectedResultsTag}";
        string aRunHistoryHeader = $"## {RunHistoryTag}";
        string aInternalProgramDataHeader = $"#### {InternalProgramDataTag}";


        string aTitleAndMetaRegion = GetRegion(aLines, $"# {TCPrefix}", aDescHeader, ReadExceptionTypes.MissingTestTitleAndMetaRegion);
        ExtractTestIDAndTitle(aTitleAndMetaRegion, aModel);
        ExtractMetaData(aTitleAndMetaRegion, aModel);

        string aDescriptionRegion = GetRegion(aLines, aDescHeader, aPreHeader, ReadExceptionTypes.MissingTestDescriptionRegion);
        aModel.Description = aDescriptionRegion.Replace(aDescHeader, string.Empty).Trim();

        string aPreconditionsRegion = GetRegion(aLines, aPreHeader, aDataHeader, ReadExceptionTypes.MissingTestPreconditionRegion);
        aModel.Precondition = aPreconditionsRegion.Replace(aPreHeader, string.Empty).Trim();

        string aDatasetRegion = GetRegion(aLines, aDataHeader, aStepsAndResultsHeader, ReadExceptionTypes.MissingTestDatasetRegion);
        aModel.TestData = aDatasetRegion.Replace(aDataHeader, string.Empty).Trim();

        string aStepsRegion = GetRegion(aLines, aStepsAndResultsHeader, aRunHistoryHeader, ReadExceptionTypes.MissingTestStepsAndExpectedRegion);
        ExtractStepResultsTable(aStepsRegion, aModel);


        string aRunHistoryRegion = GetRegion(aLines, aRunHistoryHeader, aInternalProgramDataHeader, ReadExceptionTypes.MissingRunHistoryRegion);
        ExtractAllTestRunHistory(aRunHistoryRegion, aModel);
        return aModel;
    }

    private void ExtractStepForRunHistory(string theStepsRegion, TestCaseModel theModel, DateOnly theDate)
    {
        string[] theLines = theStepsRegion.Split(NewLineExtensions.EOLC);
        List<List<string>> aTableColumns = new List<List<string>>();
        GetTableData(theLines, aTableColumns);
        if (!aTableColumns.Any())
        {
            return;
        }

        if (aTableColumns[0].Count != 3)
        {
            throw new TestCaseReadException(ReadExceptionTypes.CorruptTestStepsAndResultsTable);
        }

        var aTestRun = new TestRun(theDate);
        foreach (List<string> aRow in aTableColumns)
        {
            if (!int.TryParse(aRow[0], out int aRowIndex))
            {
                throw new TestCaseReadException(ReadExceptionTypes.CorruptTestRunHistoryRow, string.Join(',', aRow));
            }

            if (!Enum.TryParse(aRow[1], out TestStepStatus aStatus))
            {
                throw new TestCaseReadException(ReadExceptionTypes.CorruptTestRunHistoryRow, string.Join(',', aRow));
            }

            aTestRun.Results.Add(new TestStepResultModel(aRowIndex, aStatus) { Note = aRow[2] });
        }

        theModel.TestRuns.Add(aTestRun);
    }

    private void ExtractAllTestRunHistory(string theHistoryRegion, TestCaseModel theModel)
    {
        string[] aLines = theHistoryRegion.Split(NewLineExtensions.EOLC);
        List<string> aRunMarkers = new List<string>();
        foreach (string aLine in aLines)
        {
            if (aLine.StartsWith($"### {RunPrefix}", StringComparison.Ordinal))
            {
                aRunMarkers.Add(aLine);
            }
        }

        if(!aRunMarkers.Any())
        {
            return;
        }

        for (int i = 0; i < aRunMarkers.Count - 1; i++)
        {
            string aStepsRegion = GetRegion(aLines, aRunMarkers[i], aRunMarkers[i + 1], ReadExceptionTypes.MissingRunHistoryRegion);
            ExtractStepForRunHistory(aStepsRegion, theModel, ReadOutDateFromHistoryTitle(aRunMarkers[i]));
        }

        string aLeftOut = aRunMarkers.Last();
        var aBuilder = new StringBuilder();
        bool aStartAppending = false;
        foreach (string aLine in aLines)
        {
            if (aLine == aLeftOut)
            {
                aStartAppending = true;
            }

            if (aStartAppending)
            {
                aBuilder.Append($"{aLine}{NewLineExtensions.EOLC}");
            }
        }

        ExtractStepForRunHistory(aBuilder.ToString(), theModel, ReadOutDateFromHistoryTitle(aLeftOut));
    }
    
    private static DateOnly ReadOutDateFromHistoryTitle(string theLine)
    {

        string []aParts = theLine.Split(':', StringSplitOptions.RemoveEmptyEntries);
        if (aParts.Length != 2)
        {
            throw new TestCaseReadException(ReadExceptionTypes.CorruptTestRunTitleCorrupt, theLine);
        }

        if (!DateOnly.TryParseExact(aParts[1].Trim(), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly aDate))
        {
            throw new TestCaseReadException(ReadExceptionTypes.CorruptTestRunTitleCorrupt, theLine);
        }
        return aDate;
    }


    public TestCaseTarget Write(TestCaseModel theModelToSave)
    {
        myDocument.Append(new MarkdownHeader($"{TCPrefix}{theModelToSave.Id}: {theModelToSave.Name}", 1));

        myDocument.Append(new MarkdownBlockquote(
            new MarkdownParagraph($"{AuthorTag}: {theModelToSave.MetaData.Author}"),
            new MarkdownParagraph($"{DateTag}: {theModelToSave.MetaData.Date.ToString(DateFormat)}"),
            new MarkdownParagraph($"{TagsTag}: {string.Join(',', theModelToSave.MetaData.Tags)}"),
            new MarkdownParagraph($"{RevisionTag}: {theModelToSave.MetaData.Revision.ToString(CultureInfo.InvariantCulture)}"),
            new MarkdownParagraph($"{StatusTag}: {theModelToSave.MetaData.Status}")
        ));

        myDocument.Append(new MarkdownHeader(DescriptionTag, 2));
        myDocument.AppendParagraph(theModelToSave.Description);

        myDocument.Append(new MarkdownHeader(PreconditionsTag, 2));
        myDocument.AppendParagraph(theModelToSave.Precondition);

        myDocument.Append(new MarkdownHeader(DatasetTag, 2));
        myDocument.AppendParagraph(theModelToSave.TestData);

        {
            myDocument.Append(new MarkdownHeader(StepsAndExpectedResultsTag, 2));
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
            myDocument.Append(new MarkdownHeader(RunHistoryTag, 2));
            int aRunNumber = theModelToSave.TestRuns.Count;
            foreach (TestRun aRun in theModelToSave.TestRuns)
            {
                myDocument.Append(new MarkdownHeader($"{RunPrefix}{aRunNumber.ToString(CultureInfo.InvariantCulture)}: {aRun.When.ToString(DateFormat)}", 3));
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

                aRunNumber--;
            }
        }

        {
            myDocument.Append(new MarkdownHeader(InternalProgramDataTag, 4));
            myDocument.Append(new MarkdownBlockquote(
                new MarkdownParagraph("Program Used: Super Simple Test Management (SSTM)"),
                new MarkdownParagraph($"SchemaVersion: {theModelToSave.SchemaVersion}")
            ));
        }

        return new TestCaseTarget(myDocument.ToPrettyString().ToUnixEol(), theModelToSave.StoragePath());
    }

    #endregion

    #region Members

    private static void ExtractTestIDAndTitle(string aTitleAndMetaRegion, TestCaseModel aModel)
    {
        string[] aLine = aTitleAndMetaRegion.Split('\n');
        if (aLine.Length < 1)
        {
            throw new TestCaseReadException(ReadExceptionTypes.MissingTestTitle);
        }

        string[] aParts = aLine[0].Split(':', 2);
        if (aParts.Length != 2)
        {
            throw new TestCaseReadException(ReadExceptionTypes.CorruptTestTitle);
        }

        if (!int.TryParse(aParts[0].Replace("#", string.Empty).Replace(TCPrefix, string.Empty).Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out int aId))
        {
            throw new TestCaseReadException(ReadExceptionTypes.CorruptTestTitle);
        }
        
        aModel.Id = aId;
        aModel.Name = aParts[1].Trim();
    }
    private static void GetTableData(string[] theLines, List<List<string>> aCols)
    {
        (List<Range> aRange, int aSkip) = GetTableRanges(theLines);
        for (int i = aSkip; i < theLines.Length; i++)
        {
            string aLine = theLines[i];
            if (string.IsNullOrWhiteSpace(aLine))
            {
                continue;
            }

            List<string> aRow = new List<string>();
            foreach (Range aR in aRange)
            {
                aRow.Add(aLine.Substring(aR.Start.Value, aR.End.Value - aR.Start.Value).Trim());
            }

            aCols.Add(aRow);
        }
    }

    private static (List<Range>, int) GetTableRanges(string[] theLines)
    {
        List<Range> aRange = [];
        int aLinesToSkip = 0;
        foreach (string aLine in theLines)
        {
            if (!aLine.StartsWith("| ---", StringComparison.InvariantCulture))
            {
                aLinesToSkip++;
                continue;
            }

            aLinesToSkip++;
            bool aRecording = false;
            int aColumn = 0;
            int aStart = 0;
            int aEnd = 0;
            for (int i = 0; i < aLine.Length; i++)
            {
                if (aLine[i] == '-')
                {
                    if (aRecording)
                    {
                        continue;
                    }

                    aRecording = true;
                    aRange.Add(new Range(i, int.MaxValue));
                }
                else
                {
                    if (!aRecording)
                    {
                        continue;
                    }

                    aRecording = false;
                    aRange[aColumn] = new Range(aRange[aColumn].Start, i);
                    aColumn++;
                }
            }

            return (aRange, aLinesToSkip);
        }

        return (aRange, aLinesToSkip);
    }

    private void ExtractMetaData(string theTitleAndMetaRegion, TestCaseModel theModel)
    {
        string[] aLines = theTitleAndMetaRegion.Split('\n');
        string[] aPrefixes = [AuthorTag, DateTag, TagsTag, RevisionTag, StatusTag];
        string[] aSuffix = [">", ">", ">", ">", ""];
        ReadExceptionTypes[] aExceptionTypes = [ReadExceptionTypes.CorruptTestAuthor, ReadExceptionTypes.CorruptTestDate, ReadExceptionTypes.CorruptTestRevision, ReadExceptionTypes.CorruptTestTags, ReadExceptionTypes.CorruptTestStatus];
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
                if (!DateOnly.TryParseExact(aParts[1].Trim(), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly aDate))
                {
                    throw new TestCaseReadException(ReadExceptionTypes.CorruptTestDate);
                }

                theModel.MetaData.Date = aDate;
            }

            if (i == 2)
            {
                foreach (string aTag in aParts[1].Trim().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    theModel.MetaData.Tags.Add(aTag);
                }
            }

            if (i == 3)
            {
                if (!int.TryParse(aParts[1].Trim(), out int aRev))
                {
                    throw new TestCaseReadException(ReadExceptionTypes.CorruptTestStatus);
                }

                theModel.MetaData.Revision = aRev;
            }

            if (i == 4)
            {
                if (!Enum.TryParse(aParts[1].Trim(), out TestCaseStatus aStatus))
                {
                    throw new TestCaseReadException(ReadExceptionTypes.CorruptTestStatus);
                }

                theModel.MetaData.Status = aStatus;
            }
        }
    }

    private void ExtractStepResultsTable(string theStepsRegion, TestCaseModel theModel)
    {
        string[] theLines = theStepsRegion.Split(NewLineExtensions.EOLC);
        List<List<string>> aTableColumns = new List<List<string>>();
        GetTableData(theLines, aTableColumns);
        if (!aTableColumns.Any())
        {
            return;
        }

        if (aTableColumns[0].Count != 3)
        {
            throw new TestCaseReadException(ReadExceptionTypes.CorruptTestStepsAndResultsTable);
        }

        foreach (List<string> aRow in aTableColumns)
        {
            if (!int.TryParse(aRow[0], out int aRowIndex))
            {
                throw new TestCaseReadException(ReadExceptionTypes.CorruptTestStepsAndResultsRow, string.Join(',', aRow));
            }

            theModel.Steps.Add(new TestStepModel(aRowIndex, aRow[1], aRow[2]));
        }
    }

    private string GetRegion(string[] theLines, string thePrefix1, string thePrefix2, ReadExceptionTypes theExceptionTypes)
    {
        int aStartIndex = -1;
        int aEndIndex = -1;
        for (int i = 0; i < theLines.Length; i++)
        {
            if (theLines[i].StartsWith(thePrefix1, StringComparison.InvariantCulture))
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

        if (aEndIndex == -1 || aEndIndex <= aStartIndex)
        {
            throw new TestCaseReadException(theExceptionTypes);
        }

        var aBuilder = new StringBuilder();
        for (int i = aStartIndex; i < aEndIndex; i++)
        {
            aBuilder.Append($"{theLines[i]}\n");
        }

        return aBuilder.ToString();
    }

    #endregion

}