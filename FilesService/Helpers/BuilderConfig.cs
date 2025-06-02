using Common.Services;
using Common.Services.Interfaces;
using FilesService.Consumers;
using FilesService.Services;
using FilesService.Services.Interface;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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
                bus.AddConsumer<DeleteFileConsumer>();
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
            builder.Services.AddSingleton<IDbContext, DbContext>();
            builder.Services.AddSingleton<IDataAccessService, DataAccessService>();
            builder.Services.AddScoped<IS3Service, S3Service>();
            builder.Services.AddScoped<IFilesService, _FilesService>();
            builder.Services.AddScoped<IMessageQueueService, MessageQueueService>(provider => new MessageQueueService
            (
                    provider.GetRequiredService<IPublishEndpoint>(),
                    provider.GetRequiredService<IScopedClientFactory>()
            ));
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
            builder.WebHost.UseKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = size;
            });
        }
    }
}
