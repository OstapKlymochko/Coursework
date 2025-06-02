using FluentValidation;
using UserService.Models;

namespace UserService.Validators
{
	public class UpdateUserValidator: AbstractValidator<UpdateUserModel>
	{
		public UpdateUserValidator()
		{
            RuleFor(u => u.Email).EmailAddress().WithMessage("Invalid email address");
            RuleFor(u => u.Email).NotNull().NotEmpty().WithMessage("Email must not be empty");
            RuleFor(u => u.UserName).NotEmpty().WithMessage("Username must not be empty").MinimumLength(3)
               .WithMessage("Username is too short").MaximumLength(50).WithMessage("Username is too long");
            When(u => u.FirstName != null && u.FirstName != string.Empty, () =>
            {
                RuleFor(u => u.FirstName)
                .MinimumLength(2).WithMessage("First name should be have least 2 symbols")
                .MaximumLength(50).WithMessage("First name is too long. 50 symbols allowed");
            });
            When(u => u.LastName != null && u.LastName != string.Empty, () =>
            {
                RuleFor(u => u.LastName)
                .MinimumLength(2).WithMessage("Last name should be have least 2 symbols")
                .MaximumLength(50).WithMessage("Last name must not be empty");
            });
            RuleFor(u => u.Id).NotEmpty().GreaterThan(0).WithMessage("Invalid user id");
        }
	}
}
