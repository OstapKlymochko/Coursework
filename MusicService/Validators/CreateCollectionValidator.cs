using MusicService.Models;
using FluentValidation;

namespace MusicService.Validators
{
	public class CreateCollectionValidator : AbstractValidator<CreateCollectionModel>
	{
		public CreateCollectionValidator()
		{
			RuleFor(c => c.Title).NotEmpty().NotNull().WithMessage("Title must not be empty");
			RuleFor(c => c.Type).Must(t => t == "Album" || t == "Playlist").WithMessage("Only albums and playlists are allowed");
		}
	}
}
