using AuthService.Models;
using AuthService.Services.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace AuthService.Services
{
	public class EmailService : IEmailService
	{
		private readonly IConfiguration _config;
		public EmailService(IConfiguration config)
		{
			_config = config;
		}
		public async Task SendEmailAsync(string message, string subject, string receiverEmail)
		{
			//Todo replace message from parameters to a static html, create model with receiverEmail options
			using MimeMessage emailMessage = new MimeMessage();
			var config = this.GetEmailConfig();
			var emailFrom = new MailboxAddress(config.SenderName, config.SenderEmail);
			var emailTo = new MailboxAddress("???", receiverEmail);
			emailMessage.From.Add(emailFrom);
			emailMessage.To.Add(emailTo);
			emailMessage.Subject = subject;

			BodyBuilder emailBodyBuilder = new BodyBuilder();
			emailBodyBuilder.HtmlBody = message;
			emailMessage.Body = emailBodyBuilder.ToMessageBody();
			using SmtpClient client = new SmtpClient();
			await client.ConnectAsync(config.Server, config.Port);
			await client.AuthenticateAsync(config.UserName, config.Password);
			await client.SendAsync(emailMessage);
			await client.DisconnectAsync(true);
		}

		private EmailConfigModel GetEmailConfig()
		{
			string key = "GoogleSMTP";
			string server = _config.GetValue<string>($"{key}:Server")!;
			int port = _config.GetValue<int>($"{key}:Port")!;
			string senderName = _config.GetValue<string>($"{key}:SenderName")!;
			string senderEmail = _config.GetValue<string>($"{key}:SenderEmail")!;
			string userName = _config.GetValue<string>($"{key}:Username")!;
			string password = _config.GetValue<string>($"{key}:Password")!;
			return new EmailConfigModel()
			{
				Server = server, Port = port, Password = password, UserName = userName, SenderEmail = senderEmail,
				SenderName = senderName
			};
		}
	}
}
