using AuthService.Controllers;
using AuthService.Models;
using AuthService.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuthService.Tests.Controllers_tests
{
    public class RolesControllerTests
    {
        private readonly RoleController _rolesController;
        private readonly Mock<IRolesService> _rolesService;

        public RolesControllerTests()
        {
            _rolesService = new Mock<IRolesService>();
            _rolesController = new RoleController(_rolesService.Object);
        }

        [Fact]
        public async Task RoleController_GetRoles()
        {
            _rolesService.Setup(r => r.GetRoles()).ReturnsAsync(new List<RoleModel>());
            var result = await _rolesController.GetRoles();

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async Task RoleController_GetRoleById()
        {
            _rolesService.Setup(r => r.GetRoleById(GetAny<string>())).ReturnsAsync(new RoleModel() { Id = 1, Name = "Rnd" });
            var result = await _rolesController.GetRoleById(1);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }
        private T GetAny<T>() => It.IsAny<T>();

    }
}
