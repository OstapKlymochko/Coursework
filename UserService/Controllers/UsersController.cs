using Microsoft.AspNetCore.Mvc;
using UserService.Models;
using UserService.Services.Interfaces;
using UserService.Helpers;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyData()
        {
            int userId = this.ExtractIdFromToken();
            bool isAuthor = User.IsInRole("Author");
            var result = await _userService.GetByIdAsync(userId);
            if (isAuthor && result.IsSuccess) result.Value!.Roles = new List<string>() { "Author" };
            return result.MapToResponse();
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(UpdateUserModel user)
        {
            bool isAuthor = User.IsInRole("Author");
            var result = await _userService.UpdateUser(user, isAuthor);
            return result.MapToResponse();
        }
    }
}
