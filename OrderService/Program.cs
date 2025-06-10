using Microsoft.EntityFrameworkCore;

using OrderService.Data;
using OrderService.Interfaces;
using OrderService.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContextPool<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrderDb"))
    );

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IOrderService, OrderService.Services.OrderService>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    DataSeeder.SeedDevelopmentData(app);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => TypedResults.Ok(new
{
    status = "ok",
    hostname = Environment.MachineName,
    version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown"
}));

app.Run();
