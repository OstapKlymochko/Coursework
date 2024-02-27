using Common.Contracts;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Models
{
	public class UserDataModel
	{
		public UserDataModel()
		{
		}
		public UserDataModel(IdentityUser<int> identityUser)
		{
			Id = identityUser.Id;
			UserName = identityUser.UserName!;
			Email = identityUser.Email!;
		}
		public UserDataModel(IdentityUser<int> identityUser, GetUserDataResponse userData): this(identityUser)
		{
			FirstName = userData.FirstName!;
			LastName = userData.LastName!;
		}

		public int Id { get; set; }
		public string UserName { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string FirstName { get; set; } = null!;
		public string LastName { get; set; } = null!;

	}
}
