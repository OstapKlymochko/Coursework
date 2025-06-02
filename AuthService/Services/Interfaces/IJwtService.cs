using AuthService.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Services.Interfaces
{
	public interface IJwtService
	{
        public TokenPairModel GenerateTokenPair(IdentityUser<int> user, IEnumerable<string?> roles);
        public string SignResetPasswordToken(IdentityUser<int> user);
		public ResetPasswordClaims? ValidateResetPasswordToken(string jwtToken);
	}
}
