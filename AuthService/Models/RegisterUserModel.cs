namespace AuthService.Models
{
	public class RegisterUserModel: LoginUserModel
	{
		public string? UserName { get; set; } = null!;
		public string Role { get; set; } = null!;
	}
}
