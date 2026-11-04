using InvoiceTracker.Domain.Common;

using Microsoft.AspNetCore.Mvc;

namespace InvoiceTracker.Api.Common;

/// <summary>
/// The base class for the API's controllers. Routes each controller under <c>api/</c>
/// and turns business errors into Problem Details responses.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiController : ControllerBase
{
    /// <summary>
    /// Turns a business error into a Problem Details response, with the status code for its type.
    /// </summary>
    /// <param name="error">The error a use case returned.</param>
    /// <returns>
    /// A 400 response with the problems for each field for a validation error, or a response with the error's
    /// message and 404 for not found, 409 for a conflict, 422 for a business rule or 500 for any other type.
    /// </returns>
    protected ActionResult Problem(Error error)
    {
        if (error.Type == ErrorType.Validation)
        {
            foreach (KeyValuePair<string, string[]> fieldError in error.FieldErrors)
            {
                foreach (var message in fieldError.Value)
                {
                    ModelState.AddModelError(fieldError.Key, message);
                }
            }

            return ValidationProblem(ModelState);
        }

        var statusCode = error.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.BusinessRule => StatusCodes.Status422UnprocessableEntity,
            _ => StatusCodes.Status500InternalServerError
        };

        return Problem(detail: error.Message, statusCode: statusCode);
    }
}
