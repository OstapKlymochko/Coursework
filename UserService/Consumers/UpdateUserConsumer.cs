using Common.Contracts;
using MassTransit;
using UserService.Models;
using UserService.Services.Interfaces;

namespace UserService.Consumers
{
	public class UpdateUserConsumer : IConsumer<UpdateUserData>
	{
		private readonly IUserService _userService;
		public UpdateUserConsumer(IUserService userService)
		{
			_userService = userService;
		}
		public async Task Consume(ConsumeContext<UpdateUserData> context)
		{
			var updatedUser = new UserModel() { FirstName = context.Message.FirstName, LastName = context.Message.LastName, Id = context.Message.Id };
			await _userService.UpdateUser(updatedUser);
		}
	}
}
