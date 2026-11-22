using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Simplifier.TestManagement.Models;
using Simplifier.TestManagement.Services;
using Simplifier.TestManagement.ViewModels;
using Simplifier.TestManagement.Validators;
using Avalonia.Controls.Notifications;
using SukiUI.Toasts;

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
        var mockToastMessenger = new Mock<IToastMessenger>();
        var mockPlatformServices = new Mock<IPlatformServices>();
        var project = TestProjectModel.FromFullPath("/home/user/TestPath");
        var tests = new List<TestCaseModel>
        {
            new TestCaseModel(project, new TestCaseMetadataModel()) { Id = 1, Name = "Test1" },
            new TestCaseModel(project, new TestCaseMetadataModel()) { Id = 2, Name = "Test2" }
        };
        mockStorageService.Setup(s => s.Load(It.IsAny<string>())).Returns(tests);
        
        var viewModel = new ProjectDetailsViewModel(mockToastMessenger.Object, mockStorageService.Object, mockRendererFactory.Object, new SharedContext(), mockPlatformServices.Object);
        
        // Act
        viewModel.LoadTests(project);
        
        // Assert
        Assert.That(viewModel.Tests.Count, Is.EqualTo(2));
        Assert.That(viewModel.Tests[0].Id, Is.EqualTo(1));
        Assert.That(viewModel.Tests[0].Name, Is.EqualTo("Test1"));
        Assert.That(viewModel.Tests[1].Id, Is.EqualTo(2));
        Assert.That(viewModel.Tests[1].Name, Is.EqualTo("Test2"));
    }

    [Test]
    public void CreateNewTestCase_ShouldAddNewTestCaseToList()
    {
        // Arrange
        var mockStorageService = new Mock<IStorageService>();
        var mockRendererFactory = new Mock<ITestCaseRendererFactory>();
        var mockToastMessenger = new Mock<IToastMessenger>();
        var mockPlatformServices = new Mock<IPlatformServices>();
        var project = TestProjectModel.FromFullPath("/home/user/TestPath");
        
        var viewModel = new ProjectDetailsViewModel(mockToastMessenger.Object, mockStorageService.Object, mockRendererFactory.Object, new SharedContext(), mockPlatformServices.Object);
        viewModel.LoadTests(project);
        
        // Act
        viewModel.CreateNewTestCaseCommand.Execute(null);
        
        // Assert
        Assert.That(viewModel.Tests.Count, Is.EqualTo(1));
        Assert.That(viewModel.SelectedTestCase, Is.Not.Null);
        Assert.That(viewModel.SelectedTestCase.Id, Is.EqualTo(1));
    }

    [Test]
    public async Task DeleteTestCase_ShouldRemoveFromListAndStorage_WhenConfirmed()
    {
        // Arrange
        var mockStorageService = new Mock<IStorageService>();
        var mockRendererFactory = new Mock<ITestCaseRendererFactory>();
        var mockToastMessenger = new Mock<IToastMessenger>();
        var mockPlatformServices = new Mock<IPlatformServices>();
        var project = TestProjectModel.FromFullPath("/home/user/TestPath");
        var testCase = new TestCaseModel(project, new TestCaseMetadataModel()) { Id = 1 };
        
        // Need to set up platform services to return true (confirmed)
        mockPlatformServices.Setup(s => s.ShowYesNoDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NotificationType>()))
            .ReturnsAsync(true);
            
        var viewModel = new ProjectDetailsViewModel(mockToastMessenger.Object, mockStorageService.Object, mockRendererFactory.Object, new SharedContext(), mockPlatformServices.Object);
        viewModel.LoadTests(project);
        
        viewModel.Tests.Add(testCase);
        viewModel.SelectedTestCase = testCase;
        
        // Act
        await viewModel.DeleteTestCaseCommand.ExecuteAsync(null);
        
        // Assert
        Assert.That(viewModel.Tests.Count, Is.EqualTo(0));
        mockStorageService.Verify(s => s.Delete(It.IsAny<string>()), Times.Once);
        mockToastMessenger.Verify(m => m.Success(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void SaveTestCase_ShouldSaveAndShowSuccess_WhenValid()
    {
        // Arrange
        TestModelDuplicateHandler.SharedContext = null;
        var mockStorageService = new Mock<IStorageService>();
        var mockRendererFactory = new Mock<ITestCaseRendererFactory>();
        var mockToastMessenger = new Mock<IToastMessenger>();
        var mockPlatformServices = new Mock<IPlatformServices>();
        
        var project = TestProjectModel.FromFullPath("/home/user/TestPath");
        var testCase = new TestCaseModel(project, new TestCaseMetadataModel()) { Id = 1, Name = "Test1" };
        
        var mockRenderer = new Mock<ITestCaseRenderer>();
        mockRenderer.Setup(r => r.Write(It.IsAny<TestCaseModel>())).Returns(new TestCaseTarget("content", "filename"));
        mockRendererFactory.Setup(f => f.Create(It.IsAny<TestCaseModel>())).Returns(mockRenderer.Object);
        
        var sharedContext = new SharedContext();
        sharedContext.OriginalTestCases[testCase.UniqueId] = testCase.DeepCopy();
        
        var viewModel = new ProjectDetailsViewModel(mockToastMessenger.Object, mockStorageService.Object, mockRendererFactory.Object, sharedContext, mockPlatformServices.Object);
        
        mockStorageService.Setup(s => s.Load(It.IsAny<string>())).Returns(new List<TestCaseModel> { testCase });
        viewModel.LoadTests(project);
        
        viewModel.SelectedTestCase = testCase;
        
        // Act
        viewModel.SaveTestCaseCommand.Execute(null);
        
        // Assert
        Assert.That(testCase.Validate(), Is.True, "TestCase should be valid before saving");
        mockToastMessenger.Verify(m => m.Success(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void AddStep_ShouldAddStepToSelectedTestCase()
    {
        // Arrange
        var mockToastMessenger = new Mock<IToastMessenger>();
        var mockStorageService = new Mock<IStorageService>();
        var mockRendererFactory = new Mock<ITestCaseRendererFactory>();
        var mockPlatformServices = new Mock<IPlatformServices>();
        var project = TestProjectModel.FromFullPath("/home/user/TestPath");
        var testCase = new TestCaseModel(project, new TestCaseMetadataModel()) { Id = 1 };
        
        var viewModel = new ProjectDetailsViewModel(mockToastMessenger.Object, mockStorageService.Object, mockRendererFactory.Object, new SharedContext(), mockPlatformServices.Object);
        viewModel.SelectedTestCase = testCase;
        
        // Act
        viewModel.AddStepCommand.Execute(null);
        
        // Assert
        Assert.That(testCase.Steps.Count, Is.EqualTo(1));
        Assert.That(testCase.Steps[0].Number, Is.EqualTo(1));
    }

    [Test]
    public void RemoveStep_ShouldRemoveStepAndRenumber()
    {
        // Arrange
        var mockToastMessenger = new Mock<IToastMessenger>();
        var mockStorageService = new Mock<IStorageService>();
        var mockRendererFactory = new Mock<ITestCaseRendererFactory>();
        var mockPlatformServices = new Mock<IPlatformServices>();
        var project = TestProjectModel.FromFullPath("/home/user/TestPath");
        var testCase = new TestCaseModel(project, new TestCaseMetadataModel()) { Id = 1 };
        testCase.Steps.Add(new TestStepModel(1, "Proc1", "Exp1"));
        testCase.Steps.Add(new TestStepModel(2, "Proc2", "Exp2"));
        
        var viewModel = new ProjectDetailsViewModel(mockToastMessenger.Object, mockStorageService.Object, mockRendererFactory.Object, new SharedContext(), mockPlatformServices.Object);
        viewModel.SelectedTestCase = testCase;
        
        // Act
        viewModel.RemoveStepCommand.Execute(testCase.Steps[0]);
        
        // Assert
        Assert.That(testCase.Steps.Count, Is.EqualTo(1));
        Assert.That(testCase.Steps[0].Number, Is.EqualTo(1));
        Assert.That(testCase.Steps[0].Procedure, Is.EqualTo("Proc2"));
        mockToastMessenger.Verify(m => m.Success(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void SaveRun_ShouldSave_WhenValid()
    {
        // Arrange
        var mockToastMessenger = new Mock<IToastMessenger>();
        var mockStorageService = new Mock<IStorageService>();
        var mockRendererFactory = new Mock<ITestCaseRendererFactory>();
        var mockPlatformServices = new Mock<IPlatformServices>();
        var project = TestProjectModel.FromFullPath("/home/user/TestPath");
        var testCase = new TestCaseModel(project, new TestCaseMetadataModel()) { Id = 1 };
        
        var sharedContext = new SharedContext();
        sharedContext.OriginalTestCases[testCase.UniqueId] = testCase.DeepCopy();
        
        var viewModel = new ProjectDetailsViewModel(mockToastMessenger.Object, mockStorageService.Object, mockRendererFactory.Object, sharedContext, mockPlatformServices.Object);
        viewModel.SelectedRunTestCase = testCase;
        viewModel.ActiveRunSteps.Add(new TestStepResultModel(1, TestStepStatus.Pass));
        
        // Need to setup renderer factory for StoreTestCase
        var mockRenderer = new Mock<ITestCaseRenderer>();
        mockRenderer.Setup(r => r.Write(It.IsAny<TestCaseModel>())).Returns(new TestCaseTarget("content", "filename"));
        mockRendererFactory.Setup(f => f.Create(It.IsAny<TestCaseModel>())).Returns(mockRenderer.Object);
        
        // Act
        viewModel.SaveRunCommand.Execute(null);
        
        // Assert
        Assert.That(testCase.TestRuns.Count, Is.EqualTo(1));
        mockToastMessenger.Verify(m => m.Success(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void OnSelectedRunTestCaseChanged_ShouldPopulateActiveRunSteps()
    {
        // Arrange
        var mockToastMessenger = new Mock<IToastMessenger>();
        var mockStorageService = new Mock<IStorageService>();
        var mockRendererFactory = new Mock<ITestCaseRendererFactory>();
        var mockPlatformServices = new Mock<IPlatformServices>();
        var project = TestProjectModel.FromFullPath("/home/user/TestPath");
        var testCase = new TestCaseModel(project, new TestCaseMetadataModel()) { Id = 1 };
        testCase.Steps.Add(new TestStepModel(1, "Proc1", "Exp1"));
        
        var viewModel = new ProjectDetailsViewModel(mockToastMessenger.Object, mockStorageService.Object, mockRendererFactory.Object, new SharedContext(), mockPlatformServices.Object);
        
        // Act
        viewModel.SelectedRunTestCase = testCase;
        
        // Assert
        Assert.That(viewModel.ActiveRunSteps.Count, Is.EqualTo(1));
        Assert.That(viewModel.ActiveRunSteps[0].Procedure, Is.EqualTo("Proc1"));
    }

    [Test]
    public void OnSelectedTabIndexChanged_ShouldUpdateIsAddStepVisible()
    {
        // Arrange
        var mockToastMessenger = new Mock<IToastMessenger>();
        var mockStorageService = new Mock<IStorageService>();
        var mockRendererFactory = new Mock<ITestCaseRendererFactory>();
        var mockPlatformServices = new Mock<IPlatformServices>();
        
        var viewModel = new ProjectDetailsViewModel(mockToastMessenger.Object, mockStorageService.Object, mockRendererFactory.Object, new SharedContext(), mockPlatformServices.Object);
        
        // Act
        viewModel.SelectedTabIndex = 1; // TestTabIndex
        
        // Assert
        Assert.That(viewModel.IsAddStepVisible, Is.True);
    }
}
