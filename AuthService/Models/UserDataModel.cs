using Common.Contracts;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Models
{
	public class UserDataModel
	{
		public UserDataModel()
		{
		}
		public UserDataModel(IdentityUser<int> identityUser, IEnumerable<string>? roles = null)
		{
			Id = identityUser.Id;
			UserName = identityUser.UserName!;
			Email = identityUser.Email!;
			Roles = roles;
		}
		public UserDataModel(IdentityUser<int> identityUser, GetUserDataResponse userData, IEnumerable<string>? roles = null) : this(identityUser, roles)
		{
			FirstName = userData.FirstName!;
			LastName = userData.LastName!;
		}
		public IEnumerable<string>? Roles { get; set; }
		public int Id { get; set; }
		public string UserName { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string FirstName { get; set; } = null!;
		public string LastName { get; set; } = null!;
	}
}
