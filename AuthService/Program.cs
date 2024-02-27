using AuthService.Helpers;
using Serilog;
using Serilog.Sinks.Datadog.Logs;

var builder = WebApplication.CreateBuilder(args);
builder.InitDbContext();
builder.InitIdentity();
builder.InitAuthorization();
builder.InitServices();
builder.InitRabbitMq();

//var datadogConfig = new DatadogConfiguration(url: "https://http-intake.logs.us5.datadoghq.com", useSSL: false, useTCP: true);
//Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console()
//	.WriteTo.DatadogLogs(builder.Configuration.GetValue<string>("Datadog:ApiKey"),
//	service: "Auth service",
//	host: "192.168.31.93",
//	configuration: datadogConfig
//	).CreateLogger();
//using (var log = new LoggerConfiguration()
//		   .WriteTo.DatadogLogs(builder.Configuration.GetValue<string>("Datadog:ApiKey"), configuration: datadogConfig)
//		   .CreateLogger())
//{
//	log.Information("TestTestTestTestTestTestTestTestTestTestTest");
//}
//builder.Host.UseSerilog();
//builder.Services.AddSerilog(Log.Logger);
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