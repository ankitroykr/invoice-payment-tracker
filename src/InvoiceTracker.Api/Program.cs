using InvoiceTracker.Api;
using InvoiceTracker.Api.Security;
using InvoiceTracker.Application;
using InvoiceTracker.Infrastructure;
using InvoiceTracker.Infrastructure.Data;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

using Scalar.AspNetCore;

using Serilog;
using Serilog.Templates;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new ExpressionTemplate(
        "[{@t:yyyy-MM-dd HH:mm:ss} {@l:t4}] {@m}{#if @tr is not null} ({@tr}){#end}\n{@x}"))
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

    builder.Services.AddControllers(options =>
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
    builder.Services.AddRouting(options => options.LowercaseUrls = true);
    builder.Services.AddProblemDetails();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

    builder.Services.AddHealthChecks()
        .AddDbContextCheck<InvoiceTrackerDbContext>(name: "database", tags: ["ready"]);

    var connectionString = builder.Configuration.GetConnectionString("InvoiceTrackerDb") ??
                           throw new InvalidOperationException("Connection string 'InvoiceTrackerDb' not found.");

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(connectionString);

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.MapInboundClaims = false;
            options.TokenValidationParameters.RoleClaimType = "roles";
        });

    builder.Services.AddAuthorizationBuilder()
        .AddFallbackPolicy("RequireAuthenticatedUser", policy => policy.RequireAuthenticatedUser())
        .AddPolicy(Policies.CanRead, policy => policy.RequireRole(AppRoles.Read, AppRoles.ReadWrite))
        .AddPolicy(Policies.CanWrite, policy => policy.RequireRole(AppRoles.ReadWrite));

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseExceptionHandler();
    app.UseStatusCodePages();

    app.UseSerilogRequestLogging(options => options.EnrichDiagnosticContext = CallerLogEnricher.Enrich);

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.MapOpenApi().AllowAnonymous();
        app.MapScalarApiReference().AllowAnonymous();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.MapHealthChecks("/health/live",
            new HealthCheckOptions { Predicate = _ => false, ResponseWriter = HealthCheckResponseWriter.WriteResponse })
        .AllowAnonymous();

    app.MapHealthChecks("/health/ready",
        new HealthCheckOptions
        {
            Predicate = healthCheck => healthCheck.Tags.Contains("ready"),
            ResponseWriter = HealthCheckResponseWriter.WriteResponse
        }).AllowAnonymous();

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
