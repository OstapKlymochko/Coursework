using AuthService.Models;
using AuthService.Services.Interfaces;
using Common.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Moq;
using AuthService;

using User = Microsoft.AspNetCore.Identity.IdentityUser<int>;
using Role = Microsoft.AspNetCore.Identity.IdentityRole<int>;
using FluentAssertions;
using Common.CommonTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static MassTransit.ValidationResultExtensions;
using Microsoft.Extensions.Configuration;

namespace AuthService.Tests.Services_tests
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<RoleManager<Role>> _roleManagerMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IMessageQueueService> _messageQueueServiceMock;
        private readonly Mock<IValidator<RegisterUserModel>> _registerUserValidatorMock;
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
        private readonly Mock<IConfiguration> _configMock;

        private readonly IAuthService _authService;

        public AuthServiceTests()
        {
            var userStore = new Mock<IUserStore<User>>();
            var roleStore = new Mock<IRoleStore<Role>>();
            _userManagerMock = new Mock<UserManager<User>>(userStore.Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                new IUserValidator<User>[0],
                new IPasswordValidator<User>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object
            );
            _roleManagerMock = new Mock<RoleManager<Role>>(roleStore.Object, null!, null!, null!, null!);
            _jwtServiceMock = new Mock<IJwtService>();
            _emailServiceMock = new Mock<IEmailService>();
            _messageQueueServiceMock = new Mock<IMessageQueueService>();
            _registerUserValidatorMock = new Mock<IValidator<RegisterUserModel>>(MockBehavior.Strict);
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();
            _configMock = new Mock<IConfiguration>();

            _authService = new Services.AuthService(
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _jwtServiceMock.Object,
                _registerUserValidatorMock.Object,
                _messageQueueServiceMock.Object,
                _emailServiceMock.Object,
                _configMock.Object
            );
        }

        [Fact]
        public async Task AuthService_RegisterUser()
        {
            var user = new RegisterUserModel() { Email = "asdqwe@gmail.com", Password = "asdQWE123@@", Role = "Author", UserName = "okl" };
            var identityUser = new User()
            {
                Email = user.Email,
                UserName = user.UserName,
            };
            //string passwordHash = _passwordHasherMock.Object.HashPassword(identityUser, user.Password);
            //identityUser.PasswordHash = passwordHash;

            _registerUserValidatorMock
                .Setup(v => v.ValidateAsync(user, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _userManagerMock.Setup(u => u.FindByEmailAsync(user.Email)).ReturnsAsync((User)null!);
            _roleManagerMock.Setup(r => r.FindByNameAsync(user.Role)).ReturnsAsync(new Role() { Name = user.Role, Id = 1 });
            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var result = await _authService.RegisterUserAsync(user);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<BasicResponse>();
        }

        [Fact]
        public async Task AuthService_Login()
        {
            LoginUserModel creds = new() { Email = "asdqwe@gmail.com", Password = "asdQWE123@@" };

            _userManagerMock.Setup(u => u.FindByEmailAsync(creds.Email)).ReturnsAsync(new User()
            {
                Email = creds.Email,
                EmailConfirmed = true
            });
            _userManagerMock.Setup(u => u.CheckPasswordAsync(GetAny<User>(), GetAny<string>())).ReturnsAsync(true);
            _userManagerMock.Setup(u => u.GetRolesAsync(GetAny<User>())).ReturnsAsync(new List<string>() { "Author" });
            _jwtServiceMock.Setup(j => j.GenerateTokenPair(GetAny<User>(), GetAny<List<string>>())).Returns(new TokenPairModel());

            var result = await _authService.LoginAsync(creds);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<TokenPairModel>();
        }

        [Fact]
        public async Task AuthService_ResetPassword()
        {
            var resetPasswordModel = new ResetPasswordModel() { Password = "asdQWE123@@", ValidationToken = "test" };

            _jwtServiceMock.Setup(j => j.ValidateResetPasswordToken(GetAny<string>())).Returns(new ResetPasswordClaims() { UserId = 1 });
            _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(new User());
            _userManagerMock.Setup(u => u.CheckPasswordAsync(GetAny<User>(), resetPasswordModel.Password)).ReturnsAsync(false);
            _userManagerMock.Setup(u => u.ResetPasswordAsync(GetAny<User>(), GetAny<string>(), resetPasswordModel.Password)).ReturnsAsync(IdentityResult.Success);

            var result = await _authService.ResetPasswordAsync(resetPasswordModel);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<BasicResponse>();
        }

        [Fact]
        public async Task AuthService_Refresh()
        {
            _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(new User() { Id = 1, EmailConfirmed = true });
            _userManagerMock.Setup(u => u.GetRolesAsync(GetAny<User>())).ReturnsAsync(new List<string>() { "Author" });
            _jwtServiceMock.Setup(j => j.GenerateTokenPair(GetAny<User>(), GetAny<List<string>>())).Returns(new TokenPairModel());

            var result = await _authService.RefreshAsync("1");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<TokenPairModel>();
        }

        [Fact]
        public async Task AuthService_ConfirmEmail()
        {
            var confirmEmailModel = new ConfirmEmailModel() { Email = "asdqwe@gmail.com", Token = "test" };

            _userManagerMock.Setup(u => u.FindByEmailAsync(confirmEmailModel.Email)).ReturnsAsync(new User()
            {
                Email = confirmEmailModel.Email,
                EmailConfirmed = true
            });
            _userManagerMock.Setup(u => u.ConfirmEmailAsync(GetAny<User>(), GetAny<string>())).ReturnsAsync(IdentityResult.Success);

            var result = await _authService.ConfirmEmailAsync(confirmEmailModel);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<BasicResponse>();
        }

        private T GetAny<T>() => It.IsAny<T>();
    }
}
