using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ApiGateway.Helpers
{
	public static class BuilderConfig
	{
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

		public static void InitOcelot(this WebApplicationBuilder builder)
		{
			string ocelotConfigFileName = builder.Environment.IsProduction() ? "ocelot_config_prod.json" : "ocelot_config.json";
			builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
				.AddJsonFile(ocelotConfigFileName, optional: false, reloadOnChange: true)
				.AddEnvironmentVariables();
		}

		public static void SetMaxBodySize(this WebApplicationBuilder builder, long size)
		{
			builder.WebHost.UseKestrel(options => {
				options.Limits.MaxRequestBodySize = size;
			});
		}
	}
}
