using AuthService.Models;
using FluentValidation;

namespace AuthService.Validators
{
	public class ConfirmEmailValidator: AbstractValidator<ConfirmEmailModel>
	{
		public ConfirmEmailValidator()
		{
			RuleFor(m => m.Email).NotEmpty().NotNull().WithMessage("Email must not be empty").EmailAddress();
			RuleFor(m => m.Token).NotEmpty().NotNull().WithMessage("Email must not be empty");
		}
	}
}
