namespace AuthService.Models
{
	public class RegisterUserModel: LoginUserModel
	{
		public string Username { get; set; } = null!;
		public string Role { get; set; } = null!;
	}
}
