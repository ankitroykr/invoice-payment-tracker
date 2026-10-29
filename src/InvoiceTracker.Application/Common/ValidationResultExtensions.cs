using FluentValidation.Results;

using InvoiceTracker.Domain.Common;

namespace InvoiceTracker.Application.Common;

/// <summary>
/// Converts FluentValidation results into domain errors.
/// </summary>
public static class ValidationResultExtensions
{
    /// <summary>
    /// Creates a validation error from a failed validation result, with the failures grouped by field name.
    /// </summary>
    /// <param name="validationResult">The failed validation result to convert.</param>
    /// <returns>
    /// An <see cref="Error"/> of type <see cref="ErrorType.Validation"/>, with the field errors grouped by property name.
    /// </returns>
    public static Error ToValidationError(this ValidationResult validationResult)
    {
        return Error.Validation(nameof(ErrorType.Validation),
            "One or more fields are invalid.",
            validationResult.ToDictionary().AsReadOnly());
    }
}
