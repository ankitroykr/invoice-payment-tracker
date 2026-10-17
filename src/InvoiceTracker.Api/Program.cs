using InvoiceTracker.Api;
using InvoiceTracker.Api.Data;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(theme: AnsiConsoleTheme.Sixteen,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:t4}] {Message:lj} ({TraceId}){NewLine}{Exception}")
    .CreateBootstrapLogger();

var applicationName = typeof(Program).Assembly.GetName().Name;

Log.Information("Starting up {ApplicationName} application", applicationName);

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddSerilog((services, loggerConfig) => loggerConfig
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddControllers();
    builder.Services.AddProblemDetails();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    builder.Services.AddHealthChecks()
        .AddDbContextCheck<InvoiceTrackerDbContext>(name: "database", tags: ["ready"]);

    var connectionString = builder.Configuration.GetConnectionString("InvoiceTrackerDb") ??
                           throw new InvalidOperationException("Connection string 'InvoiceTrackerDb' not found.");

    builder.Services.AddDbContext<InvoiceTrackerDbContext>(options => options.UseSqlServer(connectionString));

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseExceptionHandler();
    app.UseStatusCodePages();

    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
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
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "{ApplicationName} application terminated unexpectedly", applicationName);
}
finally
{
    Log.CloseAndFlush();
}
