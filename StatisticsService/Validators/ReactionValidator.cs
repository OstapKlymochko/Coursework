using FluentValidation;
using StatisticsService.Models;

namespace StatisticsService.Validators
{
	public class ReactionValidator : AbstractValidator<CreateReactionModel>
	{
		public ReactionValidator()
		{
			RuleFor(r => r.SongId).Must(i => i > 0).WithMessage("Invalid parameter value");
			RuleFor(r => r.Type).Must(t => t == "like" || t == "dislike").WithMessage("Only like or dislike is allowed");
		}
	}
}
