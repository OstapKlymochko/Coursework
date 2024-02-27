using AuthService.Models;

namespace AuthService.Services.Interfaces
{
	public interface IRolesService
	{
		public Task<List<RoleModel>> GetRoles();
		public Task<RoleModel?> GetRoleById(string roleId);
	}
}
