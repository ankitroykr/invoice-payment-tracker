using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Testcontainers.MsSql;

namespace InvoiceTracker.Api.IntegrationTests;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class InvoiceTrackerApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer =
        new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-CU27-ubuntu-22.04")
            .Build();

    public async ValueTask InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddInMemoryCollection([
                new KeyValuePair<string, string?>("ConnectionStrings:InvoiceTrackerDb",
                    _msSqlContainer.GetConnectionString())
            ]);
        });

        return base.CreateHost(builder);
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _msSqlContainer.DisposeAsync();
    }
}
