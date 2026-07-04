using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Simplifier.TestManagement.Models;
using Simplifier.TestManagement.Services;

namespace Simplifier.TestManagement.Tests.Services;

[TestFixture]
public class StorageServiceTest
{
    private string _tempDir;
    private ITestCaseRendererFactory _factory;
    private StorageServiceImpl _service;

    [SetUp]
    public void SetUp()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
        _factory = new TestCaseRendererFactory();
        _service = new StorageServiceImpl(_factory);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, true);
        }
    }

    [Test]
    public void Save_WritesFileCorrectly()
    {
        string aPath = Path.Combine(_tempDir, "test.txt");
        string aContent = "Hello World";
        
        bool aResult = _service.Save(aPath, aPath, aContent);
        
        Assert.That(aResult, Is.True);
        Assert.That(File.Exists(aPath), Is.True);
        Assert.That(File.ReadAllText(aPath), Is.EqualTo(aContent));
    }
    
    [Test]
    public void Load_ReadsFilesCorrectly()
    {
        // Setup a valid test case file
        string aFilePath = Path.Combine(_tempDir, "TC_1_Test_Case.md");
        // We need a content that `MarkdownReadAndWriteImplV1` can parse.
        // Looking at `MarkdownReadAndWriteImplV1Test.cs`, it requires:
        // > SchemaVersion: SSTM1 on the last line
        // and appropriate headers.
        
        string aContent = @"
# TC-1: Test Case
> Author: Writer
>
> Date: 01-Jan-2020
>
> Tags: Tag1,Tag2
>
> Status: Draft

## Description
Description
## Preconditions
Preconditions
## Dataset
Dataset
## Steps & Expected Results
| Step | Procedure | Expected Result |
| ---- | --------- | --------------- |
| 1    | Proc      | Result          |
## Run History
### Run-2: 01-Jan-2021
| 1 | Pass | |

### Run-1: 01-Jan-2020
| 1 | Pass | |

### Run-Dummy: 01-Jan-2019

#### Internal Program Data [DO NOT CHANGE OR REMOVE]
> SchemaVersion: SSTM1";
        
        File.WriteAllText(aFilePath, aContent);
        
        var aResults = _service.Load(_tempDir).ToList();
        
        Assert.That(aResults.Count, Is.EqualTo(1));
        Assert.That(aResults[0].Id, Is.EqualTo(1));
        Assert.That(aResults[0].Name, Is.EqualTo("Test Case"));
    }

    [Test]
    public void Delete_RemovesFileCorrectly()
    {
        string aFilePath = Path.Combine(_tempDir, "test.md");
        File.WriteAllText(aFilePath, "content");
        
        _service.Delete(aFilePath);
        
        Assert.That(File.Exists(aFilePath), Is.False);
    }
}
