using FluentValidation;
using StatisticsService.Models;

namespace StatisticsService.Validators
{
	public class UpdateCommentValidator : AbstractValidator<UpdateCommentModel>
	{
		public UpdateCommentValidator()
		{
			RuleFor(c => c.CommentId).Must(i => i >= 0).WithMessage("Invalid parameter value");
			RuleFor(c => c.Body).NotEmpty().NotNull().WithMessage("Body must not be empty");
		}
	}
}
