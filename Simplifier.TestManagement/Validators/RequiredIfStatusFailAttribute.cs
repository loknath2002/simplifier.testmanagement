// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System.ComponentModel.DataAnnotations;
using Simplifier.TestManagement.Models;

namespace Simplifier.TestManagement.Validators;

/// <summary>
/// Validates that the decorated string property is non-empty when the
/// <see cref="TestStepResultModel.Status"/> of the owning object is
/// <see cref="TestStepStatus.Fail"/>.
/// </summary>
public class RequiredIfStatusFailAttribute : ValidationAttribute
{
    public RequiredIfStatusFailAttribute()
        : base("Note is required when the step status is Fail.")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (validationContext.ObjectInstance is TestStepResultModel model
            && model.Status == TestStepStatus.Fail)
        {
            if (value is not string note || string.IsNullOrWhiteSpace(note))
            {
                return new ValidationResult(
                    ErrorMessage ?? "Note is required when the step status is Fail.",
                    new[] { validationContext.MemberName ?? nameof(TestStepResultModel.Note) });
            }
        }

        return ValidationResult.Success;
    }
}

