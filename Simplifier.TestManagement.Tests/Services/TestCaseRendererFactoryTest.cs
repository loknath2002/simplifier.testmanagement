using System;
using NUnit.Framework;
using Simplifier.TestManagement.Models;
using Simplifier.TestManagement.Services;

namespace Simplifier.TestManagement.Tests.Services;

[TestFixture]
public class TestCaseRendererFactoryTest
{
    [Test]
    public void Create_FromModel_ReturnsMarkdownV1_ForSSTM1()
    {
        var factory = new TestCaseRendererFactory();
        var model = new TestCaseModel(TestProjectModel.FromFullPath("TestPath"), new TestCaseMetadataModel());
        
        var renderer = factory.Create(model);
        
        Assert.That(renderer, Is.TypeOf<MarkdownReadAndWriteImplV1>());
    }
    
    [Test]
    public void Create_FromTarget_ReturnsMarkdownV1_ForSSTM1()
    {
        var factory = new TestCaseRendererFactory();
        var content = "> SchemaVersion: SSTM1";
        var target = new TestCaseTarget(content, "test.md");
        
        var renderer = factory.Create(target);
        
        Assert.That(renderer, Is.TypeOf<MarkdownReadAndWriteImplV1>());
    }
    
    [Test]
    public void Create_FromTarget_Throws_ForUnsupportedVersion()
    {
        var factory = new TestCaseRendererFactory();
        var content = "> SchemaVersion: SSTM2";
        var target = new TestCaseTarget(content, "test.md");
        
        Assert.Throws<NotSupportedException>(() => factory.Create(target));
    }
    
    [Test]
    public void Create_FromTarget_Throws_If_SSTM1_Not_In_Last_Line()
    {
        var factory = new TestCaseRendererFactory();
        var content = "> SchemaVersion: SSTM1\nSomething else";
        var target = new TestCaseTarget(content, "test.md");
        
        Assert.Throws<NotSupportedException>(() => factory.Create(target));
    }

    [Test]
    public void Create_FromTarget_Works_If_SSTM1_Is_Last_Line_With_Trailing_Newline()
    {
        var factory = new TestCaseRendererFactory();
        var content = "> SchemaVersion: SSTM1\n";
        var target = new TestCaseTarget(content, "test.md");
        
        var renderer = factory.Create(target);
        
        Assert.That(renderer, Is.TypeOf<MarkdownReadAndWriteImplV1>());
    }
}
