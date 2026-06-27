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
    CorruptTestStatus,
}
public class TestCaseReadException : Exception
{
    public TestCaseReadException(ReadExceptionTypes theExceptionTypes) : base($"Test case read exception: {theExceptionTypes}")
    {
        
    }
}