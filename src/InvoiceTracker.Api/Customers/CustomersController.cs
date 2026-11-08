using InvoiceTracker.Api.Common;
using InvoiceTracker.Api.Security;
using InvoiceTracker.Application.Customers;
using InvoiceTracker.Application.Customers.CreateCustomer;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceTracker.Api.Customers;

/// <summary>
/// Creates customers and gets them by their identifier.
/// </summary>
/// <param name="createCustomerHandler">Creates a customer from the request.</param>
/// <param name="customerQueries">Reads a customer for the response.</param>
public sealed class CustomersController(CreateCustomerHandler createCustomerHandler, ICustomerQueries customerQueries)
    : ApiController
{
    // No <param> for cancellationToken: .NET 10 shows its text as the request body's description
    // (https://github.com/dotnet/aspnetcore/issues/65805). Put it back once the .NET version this
    // project runs on includes the fix.
    /// <summary>
    /// Creates a customer.
    /// </summary>
    /// <param name="request">The customer's name and email address.</param>
    /// <response code="201">The customer was created. The Location header points to it.</response>
    /// <response code="400">The request is invalid. The problems are listed for each field.</response>
    /// <response code="401">There is no access token, or it is invalid or expired.</response>
    /// <response code="403">The token doesn't have the InvoiceTracker.ReadWrite role.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden, "application/problem+json")]
    [Authorize(Policy = Policies.CanWrite)]
    public async Task<ActionResult> CreateCustomer(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var result = await createCustomerHandler.HandleAsync(request.ToCommand(), cancellationToken);

        if (result.IsFailure)
        {
            return Problem(result.Error!);
        }

        return CreatedAtAction(nameof(GetCustomer), new { id = result.Value }, null);
    }

    /// <summary>
    /// Gets a customer by their identifier.
    /// </summary>
    /// <param name="id">The customer's identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <response code="200">The customer's identifier, name and email address.</response>
    /// <response code="401">There is no access token, or it is invalid or expired.</response>
    /// <response code="403">The token doesn't have the InvoiceTracker.Read or InvoiceTracker.ReadWrite role.</response>
    /// <response code="404">There is no customer with that identifier.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType<CustomerResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    [Authorize(Policy = Policies.CanRead)]
    public async Task<ActionResult<CustomerResponse>> GetCustomer(int id, CancellationToken cancellationToken)
    {
        var customer = await customerQueries.GetCustomerByIdAsync(id, cancellationToken);

        if (customer is null)
        {
            return NotFound();
        }

        return Ok(customer);
    }
}
