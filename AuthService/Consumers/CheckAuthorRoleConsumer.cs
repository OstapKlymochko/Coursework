using Common.Contracts;
using Common.Errors;
using MassTransit;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Consumers
{
	public class CheckAuthorRoleConsumer: IConsumer<CheckUserRoleContract>
	{
		private readonly UserManager<IdentityUser<int>> _userManager;
		public CheckAuthorRoleConsumer(UserManager<IdentityUser<int>> userManager)
		{
			_userManager = userManager;
		}
		public async Task Consume(ConsumeContext<CheckUserRoleContract> context)
		{
			var user = await _userManager.FindByIdAsync(context.Message.UserId.ToString());
			if (user == null)
			{
				await context.RespondAsync<NotFoundError>(new NotFoundError("User does not exist"));
				return;
			}
			var roles = await _userManager.GetRolesAsync(user);
			await context.RespondAsync<CheckUserRoleResponse>(new CheckUserRoleResponse()
				{ IsAuthor = roles.Contains("Author") });
		}
	}
}
