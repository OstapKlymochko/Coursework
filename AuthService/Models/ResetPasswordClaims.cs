namespace AuthService.Models
{
	public class ResetPasswordClaims
	{
		public int? UserId { get; set; }
		public string TokenPurpose { get; set; } = null!;
	}
}
