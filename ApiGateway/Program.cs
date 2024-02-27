using ApiGateway.Helpers;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.InitJwt();
builder.InitOcelot();
builder.SetMaxBodySize(100 * 1024 * 1024);
builder.Services.AddControllers();
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.UseCors(options =>
{
	options.AllowAnyOrigin()
		.AllowAnyMethod()
		.AllowAnyHeader();
});

await app.UseOcelot();
app.Run();
