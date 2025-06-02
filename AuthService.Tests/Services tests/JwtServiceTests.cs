using AuthService.Models;
using AuthService.Services;
using AuthService.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using IConfigurationSection = Microsoft.Extensions.Configuration.IConfigurationSection;


namespace AuthService.Tests.Services_tests
{
    public class JwtServiceTests
    {
        private readonly IJwtService _jwtService;

        public JwtServiceTests()
        {
            _jwtService = new JwtService("asdqweasdqweasdqweasdqweasdqweasdqweasdqweasdqweasdqweasdqweasdqweasdqwe");
        }

        [Fact]
        public async Task JwtServiceTests_GenerateTokenPair()
        {
            var identityUser = new IdentityUser<int>()
            {
                Id = 1
            };
            var roles = new string[] { "Author" };
            var result = _jwtService.GenerateTokenPair(identityUser, roles);
            
            result.Should().NotBeNull();
            result.Should().BeOfType<TokenPairModel>();
            result.AccessToken.Should().NotBeEmpty();
            result.RefreshToken.Should().NotBeEmpty();
        }

        [Fact]
        public async Task JwtService_SignResetPasswordToken_ValidateResetPasswordToken()
        {
            var user = new IdentityUser<int>()
            {
                Id = 1
            };

            string token = _jwtService.SignResetPasswordToken(user);
            token.Should().NotBeNull();
            token.Should().NotBeEmpty();

            var claims = _jwtService.ValidateResetPasswordToken(token);
            claims.Should().NotBeNull();
            claims.Should().BeOfType<ResetPasswordClaims>();
            claims.UserId.Should().Be(user.Id);


        }

        private T GetAny<T>() => It.IsAny<T>();
    }
}
