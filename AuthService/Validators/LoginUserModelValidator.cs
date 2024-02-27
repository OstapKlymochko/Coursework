using AuthService.Models;
using FluentValidation;

namespace AuthService.Validators
{
	public class LoginUserModelValidator : AbstractValidator<LoginUserModel>
	{
		public LoginUserModelValidator()
		{
			RuleFor(u => u.Email).EmailAddress().WithMessage("Invalid email address");
			RuleFor(u => u.Email).NotNull().NotEmpty().WithMessage("Email must not be empty");
			RuleFor(u => u.Password).NotEmpty().WithMessage("Password must not be empty");
		}
	}
}
