namespace AuthService.Models
{
	public class TokenPairModel
	{
		public string AccessToken { get; set; } = null!;
		public string RefreshToken { get; set; } = null!;
	}
}
