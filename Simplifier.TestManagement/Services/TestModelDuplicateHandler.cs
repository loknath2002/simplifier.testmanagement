using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Simplifier.TestManagement.Models;
namespace Simplifier.TestManagement.Services;

public static class TestModelDuplicateHandler
{
    public static ISharedContext? SharedContext { get; set; }
    
    public static IEnumerable<TestCaseModel> ExistingTests => SharedContext?.OriginalTestCases.Values ?? Enumerable.Empty<TestCaseModel>();

    public static ValidationResult? ValidateForTestId(int testId, ValidationContext context)
    {
        var instance = context.ObjectInstance as TestCaseModel;
        if (instance == null)
        {
            return ValidationResult.Success;
        }

        if (ExistingTests.Any(t => t.Id == testId && t.UniqueId != instance.UniqueId))
        {
            return new ValidationResult("Test ID already exists.", new[] { nameof(TestCaseModel.Id) });
        }

        return ValidationResult.Success;
    }
}
