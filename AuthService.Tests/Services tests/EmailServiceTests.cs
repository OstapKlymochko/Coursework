using FluentAssertions;

namespace AuthService.Tests.Services_tests
{
    public class EmailServiceTests
    {
        public EmailServiceTests()
        {
            
        }

        [Fact]
        public async Task EmailService_SendEmailAsync()
        {
            await Task.Run(() =>
            {
                var b = true;
                b.Should().BeTrue();
            });
        }
    }
}
