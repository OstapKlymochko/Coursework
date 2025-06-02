using AuthService.Models;
using AuthService.Services;
using AuthService.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Role = Microsoft.AspNetCore.Identity.IdentityRole<int>;


namespace AuthService.Tests.Services_tests
{
    public class RolesServiceTest
    {
        private readonly Mock<RoleManager<Role>> _roleManagerMock;
        private readonly IRolesService _rolesService;
        public RolesServiceTest()
        {
            var roleStoreMock = new Mock<IRoleStore<Role>>();
            _roleManagerMock = new Mock<RoleManager<Role>>(roleStoreMock.Object, null!, null!, null!, null!);
            
            _rolesService = new RolesService(_roleManagerMock.Object);
        }

        [Fact]
        public async Task RolesService_GetbyId_Test()
        {
            var role = new RoleModel("User");

            _roleManagerMock.Setup(r => r.FindByIdAsync("1")).ReturnsAsync(new Role(role.Name));
            var roleModel = await _rolesService.GetRoleById("1");

            roleModel.Should().NotBeNull();
            roleModel!.Name.Should().BeEquivalentTo(role.Name);
        }

    }
}
