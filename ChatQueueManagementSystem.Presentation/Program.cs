using ChatQueueManagementSystem.Application.Extensions;
using ChatQueueManagementSystem.Infrastructure.Configurations;
using ChatQueueManagementSystem.Infrastructure.Settings;
using ChatQueueManagementSystem.Persistence.Context;
using ChatQueueManagementSystem.Persistence.Extensions;
using ChatQueueManagementSystem.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration, "Serilog")
	.Enrich.FromLogContext()
	.MinimumLevel.Information()
	.WriteTo.Console()
	.WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
	.WriteTo.Seq(builder.Configuration["SEQ_URL"]!)
	.CreateLogger();

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<OfficeHours>(builder.Configuration.GetSection("OfficeHours"));

builder.Services.AddPersistenceServices(builder.Configuration);
await builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<ChatQueueDbContext>();

	if (dbContext.Database.GetPendingMigrations().Any())
	{
		await dbContext.Database.MigrateAsync();
	}

	var seeder = scope.ServiceProvider.GetRequiredService<ChatQueueDbContextSeeder>();
	await seeder.SeedAsync();
}

if (app.Environment.IsDevelopment() || app.Environment.IsProduction()) // Add swagger in production if needed
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
	Log.Information("Starting web host");
	app.Run();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
	Log.CloseAndFlush();
}