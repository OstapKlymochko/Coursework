using AuthService.Models;
using FluentValidation;

namespace AuthService.Validators
{
	public class RegisterUserModelValidator: AbstractValidator<RegisterUserModel>
	{
		public RegisterUserModelValidator()
		{
			RuleFor(u => u.Email).EmailAddress().WithMessage("Invalid email address");
			RuleFor(u => u.Email).NotNull().NotEmpty().WithMessage("Email must not be empty");
			RuleFor(u => u.Password).NotEmpty().WithMessage("Password must not be empty");
			RuleFor(u => u.Role).NotEmpty().WithMessage("Role must not be empty");
			RuleFor(u => u.UserName
			).NotEmpty().WithMessage("Username must not be empty").MinimumLength(3)
				.WithMessage("Username is too short").MaximumLength(50).WithMessage("Username is too long");
		}
	}
}
