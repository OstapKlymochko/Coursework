using System.Text;
using AuthService.DbContext;
using AuthService.Models;
using AuthService.Services;
using AuthService.Services.Interfaces;
using AuthService.Validators;
using Common.Services.Interfaces;
using Common.Services;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using _AuthService = AuthService.Services.AuthService;
using AuthService.Consumers;

namespace AuthService.Helpers
{
    public static class BuilderConfig
	{
		public static void InitDbContext(this WebApplicationBuilder builder)
		{
			builder.Services.AddDbContext<AuthDbContext>(options =>
			{
				options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
			});

		}

		public static void InitIdentity(this WebApplicationBuilder builder)
		{
			builder.Services.AddIdentity<IdentityUser<int>, IdentityRole<int>>(options =>
			{
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireUppercase = true;
				options.Password.RequiredLength = 8;

				options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._-/ ";
			}).AddEntityFrameworkStores<AuthDbContext>().AddErrorDescriber<CustomIdentityErrorDescriber>().AddDefaultTokenProviders();
		}

		public static void InitAuthorization(this WebApplicationBuilder builder)
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

		public static void InitRabbitMq(this WebApplicationBuilder builder)
		{
			builder.Services.AddMassTransit(bus =>
			{
				var host = "Host";
                bus.SetKebabCaseEndpointNameFormatter();
				bus.AddConsumer<AuthDataUpdatedConsumer>();
				bus.UsingRabbitMq((context, config) =>
				{
					//config.Host(builder.Configuration[$"RabbitMQ:{host}"], builder.Configuration[$"RabbitMQ:Port"], h =>
					config.Host(new Uri(builder.Configuration["RabbitMQ:Uri"]!), h =>
					{
						h.Username(builder.Configuration["RabbitMQ:Username"]!);
						h.Password(builder.Configuration["RabbitMQ:Password"]!);
					});
					config.ConfigureEndpoints(context);
				});
			});
		}

		public static void InitServices(this WebApplicationBuilder builder)
		{
			ValidatorOptions.Global.LanguageManager.Enabled = false;
			builder.Services.AddScoped<IAuthService, _AuthService>();
			builder.Services.AddScoped<IEmailService, EmailService>();
			builder.Services.AddScoped<IJwtService, JwtService>();
			builder.Services.AddScoped<IRolesService, RolesService>();
			builder.Services.AddScoped<IValidator<RegisterUserModel>, RegisterUserModelValidator>();
			builder.Services.AddScoped<IMessageQueueService, MessageQueueService>(provider => new MessageQueueService
			(
				provider.GetRequiredService<IPublishEndpoint>(),
				provider.GetRequiredService<IScopedClientFactory>()
			));
		}

		public static void InitExceptionHandling(this WebApplication app)
		{
			app.UseExceptionHandler("/error");
		}

		public static void ApplyMigration(this WebApplication app)
		{
			using var scope = app.Services.CreateScope();
			var services = scope.ServiceProvider;

			var context = services.GetRequiredService<AuthDbContext>();
			if (context.Database.GetPendingMigrations().Any())
			{
				context.Database.Migrate();
			}
		}

	}
}
