using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;

using OrderService.Data;
using OrderService.Dtos;
using OrderService.Interfaces;
using OrderService.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(opts =>
    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
    );

builder.Services.AddDbContextPool<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrderDb"))
    );

builder.Services.AddHealthChecks();
builder.Services.AddHttpLogging(logging => logging.LoggingFields =
        HttpLoggingFields.RequestMethod |
        HttpLoggingFields.RequestPath |
        HttpLoggingFields.RequestQuery |
        HttpLoggingFields.ResponseStatusCode
    );

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (!builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddJsonConsole();
}

builder.Services.AddScoped<IOrderService, OrderService.Services.OrderService>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    DataSeeder.SeedDevelopmentData(app);
}

app.UseHttpLogging();

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
