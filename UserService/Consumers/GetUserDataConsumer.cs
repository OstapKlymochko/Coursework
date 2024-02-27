using Common.Contracts;
using Common.Errors;
using MassTransit;
using UserService.Services.Interfaces;

namespace UserService.Consumers
{
	public class GetUserDataConsumer : IConsumer<GetUserData>
	{
		private readonly IUserService _userService;

		public GetUserDataConsumer(IUserService userService)
		{
			_userService = userService;
		}

		public async Task Consume(ConsumeContext<GetUserData> context)
		{

			var user = await _userService.GetByIdAsync(context.Message.UserId);
			if (user == null)
			{
				await context.RespondAsync<NotFoundError>(new NotFoundError("User does not exist"));
				return;
			}
			await context.RespondAsync<GetUserDataResponse>(new GetUserDataResponse { FirstName = user.FirstName, LastName = user.LastName });
		}
	}
}
