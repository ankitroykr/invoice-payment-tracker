using InvoiceTracker.Api;
using InvoiceTracker.Api.Data;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHealthChecks();

var connectionString = builder.Configuration.GetConnectionString("InvoiceTrackerDb") ??
                       throw new InvalidOperationException("Connection string 'InvoiceTrackerDb' not found.");

builder.Services.AddDbContext<InvoiceTrackerDbContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health/live",
    new HealthCheckOptions { Predicate = _ => false, ResponseWriter = HealthCheckResponseWriter.WriteResponse });

app.MapHealthChecks("/health/ready",
    new HealthCheckOptions
    {
        Predicate = healthCheck => healthCheck.Tags.Contains("ready"),
        ResponseWriter = HealthCheckResponseWriter.WriteResponse
    });

app.Run();
