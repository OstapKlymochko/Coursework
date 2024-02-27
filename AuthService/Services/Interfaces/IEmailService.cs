namespace AuthService.Services.Interfaces
{
	public interface IEmailService
	{
		public Task SendEmailAsync(string message, string subject,string receiverEmail);
	}
}
