using AuthService.Helpers;
using AuthService.Models;
using AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var result = await _authService.RegisterUserAsync(user);
            return result.MapToResponse();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserModel user)
        {
            var result = await _authService.LoginAsync(user);
            return result.MapToResponse();
        }

        [HttpGet("refresh")]
        [Authorize]
        public async Task<IActionResult> Refresh()
        {
            string userId = this.ExtractIdFromToken().ToString();
            var result = await _authService.RefreshAsync(userId);
            return result.MapToResponse();
        }

        [HttpPost("resetPasswordRequest")]
        public async Task<IActionResult> RequestPasswordReset(UserIdentifierModel identifierModel)
        {
            var result = await _authService.RequestPasswordResetAsync(identifierModel);
            return result.MapToResponse();
        }

        [HttpPut("reset")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            var result = await _authService.ResetPasswordAsync(resetPasswordModel);
            return result.MapToResponse();
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailModel confirmModel)
        {
            var result = await _authService.ConfirmEmailAsync(confirmModel);
            return result.MapToResponse();
        }
    }
}
