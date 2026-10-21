using ApiNetCore.Api.Authentication;
using ApiNetCore.Api.Middleware;
using ApiNetCore.Application.Abstractions;
using ApiNetCore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiNetCore API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT",
        In = ParameterLocation.Header, Description = "Enter a JWT bearer token."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<AppDbContext>(
        "database",
        tags: ["ready"]);
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddSingleton<ITokenService, JwtTokenService>();
builder.Services.AddSingleton<DemoCredentialValidator>();

var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key configuration is required.");
if (jwtKey.StartsWith("CHANGE_ME", StringComparison.OrdinalIgnoreCase) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException("Set Jwt:Key to a random secret of at least 32 bytes using user secrets or environment variables.");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true, ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true, ValidateIssuerSigningKey = true, IssuerSigningKey = signingKey,
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseExceptionHandler();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Minimal API health endpoints are discoverable by Swagger.
app.MapGet("/health/live", (HealthCheckService healthChecks, CancellationToken ct) =>
        ExecuteHealthCheckAsync(healthChecks, _ => false, ct))
    .WithTags("Health")
    .WithSummary("Liveness probe")
    .Produces(StatusCodes.Status200OK)
    .AllowAnonymous();

app.MapGet("/health/ready", (HealthCheckService healthChecks, CancellationToken ct) =>
        ExecuteHealthCheckAsync(healthChecks, check => check.Tags.Contains("ready"), ct))
    .WithTags("Health")
    .WithSummary("Readiness probe, including database connectivity")
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status503ServiceUnavailable)
    .AllowAnonymous();

app.MapGet("/health", (HealthCheckService healthChecks, CancellationToken ct) =>
        ExecuteHealthCheckAsync(healthChecks, check => check.Tags.Contains("ready"), ct))
    .WithTags("Health")
    .WithSummary("Health check, including database connectivity")
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status503ServiceUnavailable)
    .AllowAnonymous();

if (app.Environment.IsDevelopment())
{
    app.MapPost("/auth/token", (TokenRequest request, DemoCredentialValidator validator, ITokenService tokens) =>
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrEmpty(request.Password))
            return Results.ValidationProblem(new Dictionary<string, string[]> { ["credentials"] = ["Username and password are required."] });
        if (!validator.IsValid(request.Username, request.Password))
            return Results.Unauthorized();
        return Results.Ok(tokens.CreateToken(request.Username));
    }).WithTags("Authentication").AllowAnonymous()
      .WithSummary("Development-only demo token endpoint")
      .WithDescription("Only available in Development. Replace with a real identity provider before production.");
}

app.MapGet("/api/todos", async (ITodoRepository repository, CancellationToken ct) =>
        Results.Ok(await repository.GetAllAsync(ct)))
    .WithTags("Todos").RequireAuthorization();

app.MapPost("/api/todos", async (CreateTodoRequest request, ITodoRepository repository, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Length > 200)
        return Results.ValidationProblem(new Dictionary<string, string[]> { ["title"] = ["Title is required and must be 200 characters or fewer."] });
    var item = new ApiNetCore.Domain.Entities.TodoItem(request.Title);
    await repository.AddAsync(item, ct);
    await repository.SaveChangesAsync(ct);
    return Results.Created($"/api/todos/{item.Id}", item);
}).WithTags("Todos").RequireAuthorization();

static async Task<IResult> ExecuteHealthCheckAsync(
    HealthCheckService healthChecks,
    Func<HealthCheckRegistration, bool> predicate,
    CancellationToken cancellationToken)
{
    var report = await healthChecks.CheckHealthAsync(predicate, cancellationToken);
    var statusCode = report.Status == HealthStatus.Healthy
        ? StatusCodes.Status200OK
        : StatusCodes.Status503ServiceUnavailable;

    var response = new
    {
        status = report.Status.ToString(),
        checks = report.Entries.ToDictionary(
            entry => entry.Key,
            entry => entry.Value.Status.ToString())
    };

    return Results.Json(response, statusCode: statusCode);
}

app.Run();

public sealed record CreateTodoRequest(string Title);
