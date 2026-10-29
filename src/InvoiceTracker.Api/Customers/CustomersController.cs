using InvoiceTracker.Api.Common;
using InvoiceTracker.Application.Customers.CreateCustomer;

using Microsoft.AspNetCore.Mvc;

namespace InvoiceTracker.Api.Customers;

public sealed class CustomersController(CreateCustomerHandler createCustomerHandler) : ApiController
{
    [HttpPost]
    public async Task<ActionResult> CreateCustomer(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var result = await createCustomerHandler.HandleAsync(request.ToCommand(), cancellationToken);

        if (result.IsFailure)
        {
            return Problem(result.Error!);
        }

        return Created();
    }
}
