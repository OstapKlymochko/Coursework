using AuthService.Services;
using AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
	[Route("api/auth/[controller]")]
	[ApiController]
	public class RoleController : ControllerBase
	{
		private readonly IRolesService _rolesService;
		public RoleController(IRolesService rolesService)
		{
			_rolesService = rolesService;
		}

		[HttpGet]
		public async Task<IActionResult> GetRoles() => Ok(await _rolesService.GetRoles());

		[HttpGet("{id}")]
		public async Task<IActionResult> GetRoleById(int id)
		{
			var role = await _rolesService.GetRoleById(id.ToString());
			if (role is null) return NotFound(new {message = "Role does not exist"});
			return Ok(role);
		}

	}
}
