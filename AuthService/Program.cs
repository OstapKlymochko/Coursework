using AuthService.Helpers;
//using Serilog;
//using Serilog.Sinks.Datadog.Logs;

var builder = WebApplication.CreateBuilder(args);
builder.InitDbContext();
builder.InitIdentity();
builder.InitAuthorization();
builder.InitServices();
builder.InitRabbitMq();

builder.Services.AddControllers();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsProduction())
{
	app.ApplyMigration();
}
app.MapControllers();
app.InitExceptionHandling();

app.Run();