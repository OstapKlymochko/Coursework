using Common.Contracts;
using MassTransit;
using UserService.Services.Interfaces;

namespace UserService.Consumers
{
    public class AvatarUploadedConsumer : IConsumer<AvatarUploadedContract>
    {
        private readonly IUserService _userService;

        public AvatarUploadedConsumer(IUserService userService)
        {
            _userService = userService;
        }

        public async Task Consume(ConsumeContext<AvatarUploadedContract> context)
        {
            var message = context.Message;
            await _userService.SetAvatarAsync(message.FileKey, message.UserId);
        }
    }
}
