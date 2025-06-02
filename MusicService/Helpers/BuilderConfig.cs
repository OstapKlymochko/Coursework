using Common.CommonTypes.Interfaces;
using Common.GenericRepository;
using Common.Services;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Common.Services.Interfaces;
using MusicService.Models;
using MusicService.Consumers;
using MusicService.Services.Interfaces;
using MusicService.Services;
using MusicService.Validators;
using Common.Contracts;

namespace MusicService.Helpers
{
    public static class BuilderConfig
    {
        public static void InitRabbitMq(this WebApplicationBuilder builder)
        {
            builder.Services.AddMassTransit(bus =>
            {
                var host = builder.Environment.IsProduction() ? "ProdHost" : "Host";
                bus.SetKebabCaseEndpointNameFormatter();
                bus.AddConsumer<PseudonymUpdatedConsumer>();
                bus.AddConsumer<MusicServiceUserRegisteredConsumer>();
                bus.AddConsumer<SongUploadedConsumer>();
                bus.AddConsumer<CommentsAvatarUploadedConsumer>();
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
            builder.Services.AddSingleton<IGenericRepository<SongDbModel>, GenericRepository<SongDbModel>>();
            builder.Services.AddSingleton<IGenericRepository<GenreDbModel>, GenericRepository<GenreDbModel>>();
            builder.Services.AddSingleton<IGenericRepository<CollectionDbModel>, GenericRepository<CollectionDbModel>>();
            builder.Services.AddScoped<IS3Service, S3Service>();
            //builder.Services.AddScoped<IFilesService, FilesService>();
            builder.Services.AddScoped<IFileLinkGeneratorService, FileLinkGeneratorService>((p) => new FileLinkGeneratorService(
                builder.Configuration["S3:ClientId"]!,
                builder.Configuration["S3:SecretKey"]!,
                builder.Configuration["S3:Bucket"]!
            ));
            builder.Services.AddScoped<ISongsService, SongsService>();
            builder.Services.AddScoped<ICollectionsService, CollectionsService>();
            builder.Services.AddScoped<ISongsDbService, SongsDbService>();
            builder.Services.AddScoped<ICollectionsDbService, CollectionsDbService>();
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
            builder.Services.AddScoped<IValidator<UploadSongModel>, UploadSongModelValidator>();
            builder.Services.AddScoped<IValidator<CreateCollectionModel>, CreateCollectionValidator>();
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
