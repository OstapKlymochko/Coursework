using AuthService.Models;
using Common.CommonTypes;
using Common.CommonTypes.Interfaces;
using Common.Result;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Services.Interfaces
{
	public interface IAuthService
	{
		public Task<ServiceResult<IBasicResponse>> RegisterUserAsync(RegisterUserModel user);
		public Task<ServiceResult<TokenPairModel>> LoginAsync(LoginUserModel userCredentials);
		public Task<ServiceResult<BasicResponse>> RequestPasswordResetAsync(UserIdentifierModel identifierModel);
		public Task<ServiceResult<BasicResponse>> ResetPasswordAsync(ResetPasswordModel resetPasswordModel);
		public Task<ServiceResult<TokenPairModel>> RefreshAsync(string userId);
		public Task<ServiceResult<BasicResponse>> ConfirmEmailAsync(ConfirmEmailModel confirmModel);
		public Task SendConfirmEmailAsync(IdentityUser<int> user);

    }
}
	