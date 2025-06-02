using AuthService.Helpers;
using AuthService.Services.Interfaces;
using Common.CommonTypes;
using Common.Contracts;
using Common.Errors;
using MassTransit;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Consumers
{
    public class AuthDataUpdatedConsumer : IConsumer<AuthDataUpdatedContract>
    {
        private readonly UserManager<IdentityUser<int>> _userManager;
        private readonly IAuthService _authService;
        public AuthDataUpdatedConsumer(UserManager<IdentityUser<int>> userManager, IAuthService authService)
        {
            _userManager = userManager;
            _authService = authService;
        }
        public async Task Consume(ConsumeContext<AuthDataUpdatedContract> context)
        {
            var message = context.Message;
            var user = await _userManager.FindByIdAsync(message.Id.ToString());
            if (user == null) return;

            bool usernameUpdated = user.Email == message.Email;

            user.Email = message.Email;
            user.UserName = message.UserName;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                await context.RespondAsync(new ModelError(result.MapErrors()));
                return;
            }

            bool isAuthor = await _userManager.IsInRoleAsync(user, "Author");
            if (isAuthor && usernameUpdated)
            {
                await context.Publish(new UsernameUpdatedContract()
                {
                    Id = message.Id,
                    IsAuthor = true,
                    UserName = message.UserName,
                });
            }
            if (message.EmailUpdated) await _authService.SendConfirmEmailAsync(user);
            await context.RespondAsync(new BasicResponse());
        }
    }
}
