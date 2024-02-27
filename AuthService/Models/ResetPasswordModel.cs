namespace AuthService.Models
{
	public class ResetPasswordModel
	{
		public string Password { get; set; } = null!;
		public string ValidationToken { get; set; } = null!;
	}
}
