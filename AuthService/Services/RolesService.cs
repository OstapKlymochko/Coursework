using AuthService.DbContext;
using AuthService.Models;
using AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Services
{
	public class RolesService : IRolesService
	{
		private readonly RoleManager<IdentityRole<int>> _roleManager;
		private readonly AuthDbContext _ctx;
		public RolesService(RoleManager<IdentityRole<int>> roleManager, AuthDbContext ctx)
		{
			_roleManager = roleManager;
			_ctx = ctx;
		}

		public async Task<List<RoleModel>> GetRoles()
		{
			return await Task.Run(() => _ctx.Roles.Select(r => new RoleModel { Id = r.Id, Name = r.Name! }).ToList());
		}

		public async Task<RoleModel?> GetRoleById(string roleId)
		{
			var role = await _roleManager.FindByIdAsync(roleId);
			if (role is null) return null;
			return new RoleModel { Id = role.Id, Name = role.Name! };
		}
	}
}
