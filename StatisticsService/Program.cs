using StatisticsService.Helpers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.InitRabbitMq();
builder.InitServices();
builder.InitAuthorization();

var app = builder.Build();
app.UseExceptionHandler("/error");
app.UseAuthorization();
app.MapControllers();
app.Run();