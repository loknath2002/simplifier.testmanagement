using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Simplifier.TestManagement.Models;
using Simplifier.TestManagement.Services;
using Simplifier.TestManagement.ViewModels;

namespace Simplifier.TestManagement.Tests.ViewModels;

[TestFixture]
public class ProjectDetailsViewModelTest
{
    [Test]
    public void LoadTests_ShouldLoadTestsFromStorageService()
    {
        // Arrange
        var mockStorageService = new Mock<IStorageService>();
        var mockRendererFactory = new Mock<ITestCaseRendererFactory>();
        var project = new TestProjectModel("TestPath");
        var tests = new List<TestCaseModel>
        {
            new TestCaseModel(project, new TestCaseMetadataModel()) { Id = 1, Name = "Test1" },
            new TestCaseModel(project, new TestCaseMetadataModel()) { Id = 2, Name = "Test2" }
        };
        mockStorageService.Setup(s => s.Load(It.IsAny<string>())).Returns(tests);
        
        var viewModel = new ProjectDetailsViewModel(mockStorageService.Object, mockRendererFactory.Object, new SharedContext());
        
        // Act
        viewModel.LoadTests(project);
        
        // Assert
        Assert.That(viewModel.Tests.Count, Is.EqualTo(2));
        Assert.That(viewModel.Tests[0].Id, Is.EqualTo(1));
        Assert.That(viewModel.Tests[0].Name, Is.EqualTo("Test1"));
        Assert.That(viewModel.Tests[1].Id, Is.EqualTo(2));
        Assert.That(viewModel.Tests[1].Name, Is.EqualTo("Test2"));
    }
}
