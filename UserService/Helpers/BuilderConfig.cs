using MassTransit;
using UserService.Consumers;
using UserService.Models;
using UserService.Repositories;
using UserService.Repositories.Interfaces;
using _UserService = UserService.Services.UserService;
using UserService.Services;
using UserService.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Common.CommonTypes.Interfaces;
using Common.GenericRepository;
using Common.Services;
using FluentValidation;
using UserService.Validators;

namespace UserService.Helpers
{
	public static class BuilderConfig
	{
		public static void InitRabbitMq(this WebApplicationBuilder builder)
		{
			builder.Services.AddMassTransit(bus=>
			{
				bus.SetKebabCaseEndpointNameFormatter(); 
				bus.AddConsumer<UserRegisteredConsumer>();
				bus.AddConsumer<GetUserDataConsumer>();
				bus.AddConsumer<UpdateUserConsumer>();
				bus.UsingRabbitMq((context, config) =>
				{
					var host = builder.Environment.IsProduction() ? "ProdHost" : "Host";
					config.Host(builder.Configuration[$"RabbitMQ:{host}"], h =>
					{
						h.Username(builder.Configuration["RabbitMQ:Username"]);
						h.Password(builder.Configuration["RabbitMQ:Password"]);
					});
					config.ConfigureEndpoints(context);
				});
			});
		}

		public static void InitServices(this WebApplicationBuilder builder)
		{
			ValidatorOptions.Global.LanguageManager.Enabled = false;
			builder.Services.AddSingleton<IDbContext, DbContext>();
			builder.Services.AddSingleton<IDataAccessService, DataAccessService>();
			builder.Services.AddSingleton<IGenericRepository<UserModel>, GenericRepository<UserModel>>();
			builder.Services.AddScoped<IUserService, _UserService>();
			builder.Services.AddScoped<IUserRepository<UserModel>, UserRepository>();
			builder.Services.AddScoped<IValidator<UserModel>, UpdateUserValidator>();
		}

		public static void InitJwt(this WebApplicationBuilder builder)
		{

			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				var encodedSecret = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!);
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(encodedSecret),
					ValidateIssuer = false,
					ValidateAudience = false,
					ClockSkew = TimeSpan.Zero
				};
			});
		}
	}
}
