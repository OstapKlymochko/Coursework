using UserService.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.InitJwt();
builder.InitRabbitMq();
builder.InitServices();
var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
