using AuthService.Models;
using AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services
{
    public class RolesService : IRolesService
    {
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        public RolesService(RoleManager<IdentityRole<int>> roleManager)
        {
            _roleManager = roleManager;
        }

        public Task<List<RoleModel>> GetRoles() =>
            _roleManager.Roles.Select(r => new RoleModel { Id = r.Id, Name = r.Name! }).ToListAsync();


        public async Task<RoleModel?> GetRoleById(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role is null) return null;
            return new RoleModel { Id = role.Id, Name = role.Name! };
        }
    }
}
