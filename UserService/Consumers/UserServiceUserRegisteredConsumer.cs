using Common.Contracts;
using MassTransit;
using UserService.Services.Interfaces;

namespace UserService.Consumers
{
    public class UserServiceUserRegisteredConsumer : IConsumer<UserRegistered>
    {
        private readonly IUserService _userService;
        public UserServiceUserRegisteredConsumer(IUserService userService)
        {
            _userService = userService;
        }
        public async Task Consume(ConsumeContext<UserRegistered> context)
        {
            var message = context.Message;
            await _userService.CreateUser(message.UserId, message.Email, message.UserName);
        }
    }
}
