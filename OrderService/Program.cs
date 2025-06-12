using System.Security.Claims;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using OrderService.Data;
using OrderService.Dtos;
using OrderService.Interfaces;
using OrderService.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string? dbConnectionString = builder.Configuration.GetConnectionString("OrderDb");
string? jwtDomain = builder.Configuration["Jwt:Domain"];
string? jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(dbConnectionString))
    throw new InvalidOperationException("Missing configuration: ConnectionStrings:OrderDb");

if (string.IsNullOrWhiteSpace(jwtDomain))
    throw new InvalidOperationException("Missing configuration: Jwt:Domain");

if (string.IsNullOrWhiteSpace(jwtAudience))
    throw new InvalidOperationException("Missing configuration: Jwt:Audience");

builder.Services.AddControllers().AddJsonOptions(opts =>
    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
    );
builder.Services.AddDbContextPool<OrderDbContext>(options =>
    options.UseNpgsql(dbConnectionString)
    );
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
{
    options.Authority = $"https://{jwtDomain}/";
    options.Audience = jwtAudience;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier
    };
});
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Order Service",
        Description = "An ASP.NET Core Web API for managing orders",
        Contact = new OpenApiContact
        {
            Name = "Uday Rana",
            Url = new Uri("https://github.com/uday-rana")
        }
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        In = ParameterLocation.Header,
        Name = "Authorization",
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter `Bearer {token}`"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference {
                Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddHttpLogging(logging =>
    logging.LoggingFields = HttpLoggingFields.RequestMethod
        | HttpLoggingFields.RequestPath
        | HttpLoggingFields.RequestQuery
        | HttpLoggingFields.ResponseStatusCode
    );
builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IOrderService, OrderService.Services.OrderService>();
if (!builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddJsonConsole();
}

WebApplication app = builder.Build();

// Apply database migrations
using (IServiceScope scope = app.Services.CreateScope())
{
    OrderDbContext db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.Migrate();
}

app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    DataSeeder.SeedDevelopmentData(app);
}
app.UseHttpsRedirection();
app.UseHttpLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, healthReport) =>
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        HealthDto dto = new(
            Status: healthReport.Status.ToString(),
            Hostname: Environment.MachineName,
            Version: typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown"
        );

        await context.Response.WriteAsJsonAsync(dto);
    }
});

app.Run();
