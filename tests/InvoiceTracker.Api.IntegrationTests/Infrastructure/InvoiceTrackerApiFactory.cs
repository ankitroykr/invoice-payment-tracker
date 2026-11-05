using System.Net.Http.Headers;
using System.Security.Cryptography;

using InvoiceTracker.Api.IntegrationTests.Infrastructure;
using InvoiceTracker.Infrastructure.Data;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using Testcontainers.MsSql;

[assembly: AssemblyFixture(typeof(InvoiceTrackerApiFactory))]

namespace InvoiceTracker.Api.IntegrationTests.Infrastructure;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class InvoiceTrackerApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer =
        new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-CU27-ubuntu-22.04")
            .Build();

    private const string TestIssuer = "https://invoicetracker.tests";
    private const string TestAudience = "api://invoicetracker-tests";

    private readonly SymmetricSecurityKey _signingKey = new(RandomNumberGenerator.GetBytes(64));

    public async ValueTask InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
        StartServer();
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<InvoiceTrackerDbContext>();
        await dbContext.Database.MigrateAsync(TestContext.Current.CancellationToken);
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

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters.ValidIssuer = TestIssuer;
                options.TokenValidationParameters.ValidAudience = TestAudience;
                options.TokenValidationParameters.IssuerSigningKey = _signingKey;
            }));

        base.ConfigureWebHost(builder);
    }

    public HttpClient CreateAuthenticatedClient(params string[] roles)
    {
        var token = CreateToken(roles);

        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);

        return client;
    }

    public string CreateToken(string[]? roles = null, string audience = TestAudience, DateTime? expires = null,
        SecurityKey? signingKey = null)
    {
        var expiresAt = expires ?? DateTime.UtcNow.AddMinutes(5);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = TestIssuer,
            Audience = audience,
            IssuedAt = expiresAt.AddMinutes(-10),
            NotBefore = expiresAt.AddMinutes(-10),
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(signingKey ?? _signingKey, SecurityAlgorithms.HmacSha512),
            Claims = new Dictionary<string, object> { ["roles"] = roles ?? [] }
        };

        var token = new JsonWebTokenHandler().CreateToken(tokenDescriptor);
        return token;
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _msSqlContainer.DisposeAsync();
    }
}
