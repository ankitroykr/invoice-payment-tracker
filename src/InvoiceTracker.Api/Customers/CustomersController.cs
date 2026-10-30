using InvoiceTracker.Api.Common;
using InvoiceTracker.Application.Customers;
using InvoiceTracker.Application.Customers.CreateCustomer;

using Microsoft.AspNetCore.Mvc;

namespace InvoiceTracker.Api.Customers;

public sealed class CustomersController(CreateCustomerHandler createCustomerHandler, ICustomerQueries customerQueries)
    : ApiController
{
    [HttpPost]
    public async Task<ActionResult> CreateCustomer(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var result = await createCustomerHandler.HandleAsync(request.ToCommand(), cancellationToken);

        if (result.IsFailure)
        {
            return Problem(result.Error!);
        }

        return CreatedAtAction(nameof(GetCustomer), new { id = result.Value }, null);
    }

    [HttpGet("{id:int}")]
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
