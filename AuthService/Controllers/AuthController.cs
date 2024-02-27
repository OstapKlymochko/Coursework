using AuthService.Helpers;
using AuthService.Models;
using AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace AuthService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterUserModel user)
		{
			var result = await _authService.RegisterUser(user);
			return result.MapToResponse();
		}
		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginUserModel user)
		{
			var result = await _authService.Login(user);
			return result.MapToResponse();
		}

		[HttpGet("me")]
		[Authorize]
		public async Task<IActionResult> GetUserData()
		{
			string userId = _authService.ExtractIdFromToken(HttpContext).ToString();
			var result = await _authService.GetUserData(userId);
			return result.MapToResponse();
		}

		[HttpGet("refresh")]
		[Authorize]
		public async Task<IActionResult> Refresh()
		{
			string userId = _authService.ExtractIdFromToken(HttpContext).ToString();
			var result = await _authService.Refresh(userId);
			return result.MapToResponse();
		}

		[HttpPost("resetPasswordRequest")]
		public async Task<IActionResult> RequestPasswordReset(UserIdentifierModel identifierModel)
		{
			var result = await _authService.RequestPasswordReset(identifierModel);
			return result.MapToResponse();
		}

		[HttpPut("reset")]
		public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
		{
			var result = await _authService.ResetPassword(resetPasswordModel);
			return result.MapToResponse();
		}
		
		[HttpPut("update")]
		[Authorize]
		public async Task<IActionResult> UpdateUser(UserDataModel updateUserData)
		{
			var userId = _authService.ExtractIdFromToken(HttpContext);
			if (userId != updateUserData.Id) return Forbid();
			var result = await _authService.UpdateUserData(updateUserData);
			return result.MapToResponse();
		}

		[HttpPost("confirm")]
		public async Task<IActionResult> ConfirmEmail(ConfirmEmailModel confirmModel)
		{
			var result = await _authService.ConfirmEmail(confirmModel);
			return result.MapToResponse();
		}
	}
}
