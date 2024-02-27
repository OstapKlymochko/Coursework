using AuthService.Models;
using FluentValidation;

namespace AuthService.Validators
{
	public class UpdateUserValidator: AbstractValidator<UserDataModel>
	{
		public UpdateUserValidator()
		{

			RuleFor(u => u.Email).EmailAddress().WithMessage("Invalid email address");
			RuleFor(u => u.Email).NotNull().NotEmpty().WithMessage("Email must not be empty");
			RuleFor(u => u.UserName).NotEmpty().WithMessage("Username must not be empty").MinimumLength(3)
				.WithMessage("Username is too short").MaximumLength(50).WithMessage("Username is too long");
			RuleFor(u => u.FirstName).NotEmpty().WithMessage("First name must not be empty")
				.MinimumLength(2).WithMessage("First name should be have least 2 symbols")
				.MaximumLength(50).WithMessage("First name is too long. 50 symbols allowed");
			RuleFor(u => u.LastName)
				.NotEmpty().WithMessage("Last name must not be empty")
				.MinimumLength(2).WithMessage("Last name should be have least 2 symbols")
				.MaximumLength(50).WithMessage("Last name must not be empty");
			RuleFor(u => u.Id).NotEmpty().GreaterThan(0).WithMessage("Invalid user id");
		}
	}
}
