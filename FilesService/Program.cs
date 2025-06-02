using FilesService.Helpers;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.InitRabbitMq();
builder.InitServices();
builder.SetMaxBodySize(100 * 1024 * 1024);
builder.InitAuthorization();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();
app.UseAuthorization();
app.UseExceptionHandler("/error");
app.MapControllers();
app.Run();