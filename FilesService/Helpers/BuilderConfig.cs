using Common.CommonTypes.Interfaces;
using Common.GenericRepository;
using Common.Services;
using FilesService.Models;
using FilesService.Services;
using FilesService.Services.Interfaces;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FilesService.Validators;
using _FilesService = FilesService.Services.FilesService;

namespace FilesService.Helpers
{
	public static class BuilderConfig
	{
		public static void InitRabbitMq(this WebApplicationBuilder builder)
		{
			builder.Services.AddMassTransit(bus =>
			{
				var host = builder.Environment.IsProduction() ? "ProdHost" : "Host";
				bus.SetKebabCaseEndpointNameFormatter();
				bus.UsingRabbitMq((context, config) =>
				{
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
			builder.Services.AddSingleton<IGenericRepository<FileDbModel>, GenericRepository<FileDbModel>>();
			builder.Services.AddScoped<IS3Service, S3Service>();
			builder.Services.AddScoped<IFilesService, _FilesService>();
			builder.Services.AddScoped<IFileDbService, FileDbService>();
			builder.Services.AddScoped<IValidator<UploadFileModel>, UploadFileModelValidator>();
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
		public static void SetMaxBodySize(this WebApplicationBuilder builder, long size)
		{
			builder.WebHost.UseKestrel(options => {
				options.Limits.MaxRequestBodySize = size;
			});
		}
	}
}
