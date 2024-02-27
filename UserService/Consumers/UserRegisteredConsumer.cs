using Common.Contracts;
using MassTransit;
using UserService.Services.Interfaces;

namespace UserService.Consumers
{
	public class UserRegisteredConsumer : IConsumer<UserRegistered>
	{
		private readonly IUserService _userService;
		public UserRegisteredConsumer(IUserService userService)
		{
			_userService = userService;
		}
		public async Task Consume(ConsumeContext<UserRegistered> context)
		{
			await _userService.CreateUser(context.Message.UserId);
		}
	}
}
