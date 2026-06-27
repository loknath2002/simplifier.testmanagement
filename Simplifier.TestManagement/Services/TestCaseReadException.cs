// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
namespace Simplifier.TestManagement.Services;

using System;

public enum ReadExceptionTypes
{
    MissingTestTitleAndMetaRegion,
    MissingTestTitle,
    CorruptTestTitle,
    CorruptTestMetadata,
    CorruptTestAuthor,
    CorruptTestDate,
    CorruptTestTags,
    CorruptTestRevision,
    CorruptTestStatus,
    MissingTestDescriptionRegion,
    MissingTestPreconditionRegion,
    MissingTestDatasetRegion,
    MissingTestStepsAndExpectedRegion,
    CorruptTestStepsAndResultsTable,
    CorruptTestStepsAndResultsRow,
    MissingRunHistoryRegion,
    CorruptTestRunHistoryRow,
    CorruptTestRunTitleCorrupt,
}
public class TestCaseReadException : Exception
{
    public TestCaseReadException(ReadExceptionTypes theExceptionTypes, string theExtraMessage) : base($"Test case read exception: {theExceptionTypes} Extra : {theExtraMessage}")
    {
        
    }
    
    public TestCaseReadException(ReadExceptionTypes theExceptionTypes) : this(theExceptionTypes, string.Empty)
    {
        
    }
}