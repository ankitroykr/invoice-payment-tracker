using InvoiceTracker.Domain.Common;

using Microsoft.AspNetCore.Mvc;

namespace InvoiceTracker.Api.Common;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiController : ControllerBase
{
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
