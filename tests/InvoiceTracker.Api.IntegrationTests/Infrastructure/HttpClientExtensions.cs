using System.Net.Http.Json;

using InvoiceTracker.Application.Customers;
using InvoiceTracker.Application.Invoices;

namespace InvoiceTracker.Api.IntegrationTests.Infrastructure;

public static class HttpClientExtensions
{
    private const string ValidCustomerName = "Test Customer";

    private const string ValidCustomerEmail = "tester@example.com";

    private const string Currency = "GBP";

    extension(HttpClient client)
    {
        public async Task<int> CreateCustomerAsync()
        {
            var response = await client.PostAsJsonAsync("api/customers",
                new { Name = ValidCustomerName, Email = ValidCustomerEmail },
                TestContext.Current.CancellationToken);

            response.EnsureSuccessStatusCode();

            var customer = await client.GetFromJsonAsync<CustomerResponse>(response.Headers.Location,
                TestContext.Current.CancellationToken);

            return customer!.Id;
        }

        public async Task<int> CreateInvoiceAsync()
        {
            var customerId = await client.CreateCustomerAsync();

            var response = await client.PostAsJsonAsync("api/invoices",
                new
                {
                    CustomerId = customerId,
                    Currency,
                    Lines = new[]
                    {
                        new { Description = "Website update and redesign", Quantity = 4m, UnitPrice = 55m }
                    }
                },
                TestContext.Current.CancellationToken);

            response.EnsureSuccessStatusCode();

            var invoice = await client.GetFromJsonAsync<InvoiceResponse>(response.Headers.Location,
                TestContext.Current.CancellationToken);

            return invoice!.Id;
        }
    }
}
