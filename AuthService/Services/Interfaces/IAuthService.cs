using AuthService.Models;
using Common.CommonTypes;
using Common.CommonTypes.Interfaces;
using Common.Result;

namespace AuthService.Services.Interfaces
{
	public interface IAuthService
	{
		public Task<ServiceResult<IBasicResponse>> RegisterUser(RegisterUserModel user);
		public Task<ServiceResult<TokenPairModel>> Login(LoginUserModel userCredentials);
		public Task<ServiceResult<BasicResponse>> RequestPasswordReset(UserIdentifierModel identifierModel);
		public Task<ServiceResult<BasicResponse>> ResetPassword(ResetPasswordModel resetPasswordModel);
		public Task<ServiceResult<UserDataModel>> GetUserData(string id);
		public Task<ServiceResult<TokenPairModel>> Refresh(string userId);
		public Task<ServiceResult<BasicResponse>> UpdateUserData(UserDataModel updateUserData);
		public Task<ServiceResult<BasicResponse>> ConfirmEmail(ConfirmEmailModel confirmModel);
		public int ExtractIdFromToken(HttpContext ctx);
	}
}
