using AuthService.Controllers;
using AuthService.Models;
using AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Common.CommonTypes;
using FluentAssertions;

namespace AuthService.Tests.Controllers_tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _authController = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task AuthController_Register()
        {
            var registerModel = new RegisterUserModel();

            _authServiceMock.Setup(a => a.RegisterUserAsync(registerModel)).ReturnsAsync(new BasicResponse(""));
            var result = await _authController.Register(registerModel);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }
        [Fact]
        public async Task AuthController_Login()
        {
            var loginModel = new LoginUserModel() { Email = "asdqwe@gmail.com", Password = "asdQWE123@@" };
            _authServiceMock.Setup(a => a.LoginAsync(loginModel)).ReturnsAsync(new TokenPairModel());

            var result = await _authController.Login(loginModel);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        //[Fact]
        //public async Task AuthController_Refresh()
        //{

        //    _authServiceMock.Setup(a => a.RefreshAsync(GetAny<string>())).ReturnsAsync(new TokenPairModel());
        //    var result = await _authController.Refresh();

        //    result.Should().NotBeNull();
        //    result.Should().BeOfType(typeof(OkObjectResult));
        //}

        [Fact]
        public async Task AuthController_RequestPasswordReset()
        {
            var identifierModel = new UserIdentifierModel() { Identifier = "asdqwe@gmail.com" };
            _authServiceMock.Setup(a => a.RequestPasswordResetAsync(identifierModel)).ReturnsAsync(new BasicResponse(""));

            var result = await _authController.RequestPasswordReset(identifierModel);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async Task AuthController_ResetPassword()
        {
            var resetPasswordModel = new ResetPasswordModel() { Password = "asdQWE123@@", ValidationToken = "test" };
            _authServiceMock.Setup(a => a.ResetPasswordAsync(resetPasswordModel)).ReturnsAsync(new BasicResponse(""));

            var result = await _authController.ResetPassword(resetPasswordModel);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }


        [Fact]
        public async Task AuthController_ConfirmEmail()
        {
            var confirmModel = new ConfirmEmailModel() { Email = "asdqwe@gmail.com", Token = "test" };
            _authServiceMock.Setup(a => a.ConfirmEmailAsync(confirmModel)).ReturnsAsync(new BasicResponse(""));

            var result = await _authController.ConfirmEmail(confirmModel);

            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        private T GetAny<T>() => It.IsAny<T>();

    }
}
