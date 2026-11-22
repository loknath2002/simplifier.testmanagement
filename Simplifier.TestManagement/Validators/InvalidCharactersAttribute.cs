using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace Simplifier.TestManagement.Validators;

public class InvalidCharactersAttribute : ValidationAttribute
{
    private readonly char[] _invalidChars;

    public InvalidCharactersAttribute(bool isPath = false)
    {
        _invalidChars = isPath ? Path.GetInvalidPathChars() : Path.GetInvalidFileNameChars();
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string stringValue)
        {
            if (stringValue.Any(c => _invalidChars.Contains(c)))
            {
                return new ValidationResult(ErrorMessage ?? "Value contains invalid characters.");
            }
        }
        
        return ValidationResult.Success;
    }
}
