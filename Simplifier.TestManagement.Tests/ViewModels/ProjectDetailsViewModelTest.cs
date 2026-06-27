using System.Collections.Generic;
using NUnit.Framework;
using Simplifier.TestManagement.Models;
using Simplifier.TestManagement.Services;
using Simplifier.TestManagement.ViewModels;

namespace Simplifier.TestManagement.Tests.ViewModels;

[TestFixture]
public class ProjectDetailsViewModelTest
{
    public class MockStorageService : IStorageService
    {
        public IEnumerable<TestCaseModel> Tests { get; set; } = new List<TestCaseModel>();
        public bool Save(string thePath, string theContent) => true;
        public IEnumerable<TestCaseModel> Load(string theProjectDirectory) => Tests;
    }

    [Test]
    public void LoadTests_ShouldLoadTestsFromStorageService()
    {
        // Arrange
        var mockStorageService = new MockStorageService();
        var project = new TestProjectModel("TestPath");
        var tests = new List<TestCaseModel>
        {
            new TestCaseModel(project, new TestCaseMetadataModel()) { Id = "1", Name = "Test1" },
            new TestCaseModel(project, new TestCaseMetadataModel()) { Id = "2", Name = "Test2" }
        };
        mockStorageService.Tests = tests;
        
        var viewModel = new ProjectDetailsViewModel(mockStorageService);
        
        // Act
        viewModel.LoadTests(project);
        
        // Assert
        Assert.That(viewModel.Tests.Count, Is.EqualTo(2));
        Assert.That(viewModel.Tests[0].Id, Is.EqualTo("1"));
        Assert.That(viewModel.Tests[0].Name, Is.EqualTo("Test1"));
        Assert.That(viewModel.Tests[1].Id, Is.EqualTo("2"));
        Assert.That(viewModel.Tests[1].Name, Is.EqualTo("Test2"));
    }
}
