using Common.Services;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Common.Services.Interfaces;
using StatisticsService.Models;
using StatisticsService.Validators;
using StatisticsService.Services.Interfaces;
using StatisticsService.Services;

namespace StatisticsService.Helpers
{
    public static class BuilderConfig
    {
        public static void InitRabbitMq(this WebApplicationBuilder builder)
        {
            builder.Services.AddMassTransit(bus =>
            {
                bus.SetKebabCaseEndpointNameFormatter();
                bus.UsingRabbitMq((context, config) =>
                {
                    config.Host(builder.Configuration[$"RabbitMQ:host"], h =>
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
            builder.Services.AddScoped<IMessageQueueService, MessageQueueService>(provider => new MessageQueueService
            (
                provider.GetRequiredService<IPublishEndpoint>(),
                provider.GetRequiredService<IScopedClientFactory>()
            ));
            builder.Services.AddScoped<ICommentsDbService, CommentsDbService>();
            builder.Services.AddScoped<ICommentsService, CommentsService>();
            builder.Services.AddScoped<IReactionsDbService, ReactionsDbService>();
            builder.Services.AddScoped<IReactionsService, ReactionsService>();
            builder.Services.AddScoped<IValidator<CreateReactionModel>, ReactionValidator>();
            builder.Services.AddScoped<IValidator<CreateCommentModel>, CommentValidator>();
            builder.Services.AddScoped<IValidator<UpdateCommentModel>, UpdateCommentValidator>();
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
    }
}
