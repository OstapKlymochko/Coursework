using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Models;
using AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services
{
	public class JwtService : IJwtService
	{
		private readonly byte[] _secretEncoded;
		public JwtService(IConfiguration config)
		{
			string secret = config.GetValue<string>("Jwt:Key")!;
			_secretEncoded = Encoding.ASCII.GetBytes(secret);
		}


		private string SignAuthToken(IdentityUser<int> user, DateTime expirationTime)
		{
			List<Claim> claims = new()
			{
				new Claim("userId", user.Id.ToString())
			};
			return this.SignToken(claims, expirationTime);
		}
		private string SignToken(List<Claim> claims, DateTime expirationTime)
		{
			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
			SymmetricSecurityKey symmetricKey = new SymmetricSecurityKey(_secretEncoded);
			SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
			{
				Subject = new ClaimsIdentity(claims),
				Expires = expirationTime,
				SigningCredentials = new(symmetricKey, SecurityAlgorithms.HmacSha256)
			};
			SecurityToken jwtToken = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(jwtToken);
		}
		private JwtSecurityToken ValidateToken(string jwtToken)
		{
			JwtSecurityTokenHandler tokenHandler = new();
			TokenValidationParameters parameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(_secretEncoded),
				ValidateIssuer = false,
				ValidateAudience = false,
				ClockSkew = TimeSpan.Zero
			};
			tokenHandler.ValidateToken(jwtToken, parameters, out SecurityToken validatedToken);
			return (JwtSecurityToken)validatedToken;
		}
		public TokenPairModel GenerateTokenPair(IdentityUser<int> user)
		{
			string accessToken = SignAuthToken(user, DateTime.UtcNow.AddDays(1));
			string refreshToken = SignAuthToken(user, DateTime.UtcNow.AddDays(2));
			return new TokenPairModel { AccessToken = accessToken, RefreshToken = refreshToken };
		}
		public string SignResetPasswordToken(IdentityUser<int> user)
		{
			List<Claim> claims = new()
			{
				new Claim("userId", user.Id.ToString()),
				new Claim("tokenPurpose", "Reset password token")
			};
			return this.SignToken(claims, DateTime.UtcNow.AddMinutes(5));
		}
		public ResetPasswordClaims? ValidateResetPasswordToken(string jwtToken)
		{
			try
			{
				var validatedJwt = this.ValidateToken(jwtToken);
				bool isParseSuccess = int.TryParse(validatedJwt.Claims.Where(c => c.Type == "userId")!.First().Value, out int userId);
				string tokenPurpose = validatedJwt.Claims.Where(c => c.Type == "tokenPurpose")!.First().Value;
				if (!isParseSuccess || userId <= 0 || tokenPurpose != "Reset password token")
					return null;

				return new ResetPasswordClaims { TokenPurpose = tokenPurpose, UserId = userId };
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return null;
			}
		}

	}
}
