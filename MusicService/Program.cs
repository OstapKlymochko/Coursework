using MusicService.Helpers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.InitRabbitMq();
builder.InitServices();
builder.SetMaxBodySize(100 * 1024 * 1024);
builder.InitAuthorization();


var app = builder.Build();
app.UseExceptionHandler("/error");
app.UseAuthentication();
app.MapControllers();
app.Run();