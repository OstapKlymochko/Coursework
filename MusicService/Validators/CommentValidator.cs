﻿using FluentValidation;
using MusicService.Models;

namespace MusicService.Validators
{
	public class CommentValidator : AbstractValidator<CreateCommentModel>
	{
		public CommentValidator()
		{
			RuleFor(c => c.SongId).Must(i => i > 0).WithMessage("Invalid parameter value");
			RuleFor(c => c.Body).NotEmpty().NotNull().WithMessage("Body must not be empty");
		}
	}
}
