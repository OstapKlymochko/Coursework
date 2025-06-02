using MusicService.Models;
using FluentValidation;

namespace MusicService.Validators
{
	public class UploadSongModelValidator : AbstractValidator<UploadSongModel>
	{
		private readonly List<string> _allowedSongContentTypes = new()
		{
			"audio/mp3",
			"audio/mpeg"
		};

		private readonly List<string> _allowedLogoContentTypes = new()
		{
			"image/jpeg",
			"image/png",
		};

		private readonly List<string> _allowedClipContentTypes = new()
		{
			"video/mp4"
		};

		public UploadSongModelValidator()
		{
			RuleFor(m => m.Title).NotEmpty().NotNull().WithMessage("Title must not be empty");
			RuleFor(m => m.GenreId).GreaterThan(0).WithMessage("Genre must be specified");
			RuleFor(m => m.Song).NotEmpty().NotNull()
				.Must(f => f.Length <= 100 * 1024 * 1024).WithMessage("File is too big")
				.Must(f => _allowedSongContentTypes.Contains(f.ContentType)).WithMessage("Invalid song file format");
			When(m => m.Logo != null, () =>
			{
				RuleFor(m => m.Logo)
					.Must(f => f!.Length <= 5 * 1024 * 1024).WithMessage("File is too big")
					.Must(f => _allowedLogoContentTypes.Contains(f!.ContentType)).WithMessage("Invalid logo file format");
			});
			When(m => m.VideoClip != null, () =>
			{
				RuleFor(m => m.VideoClip)
					.Must(f => f!.Length <= 100 * 1024 * 1024).WithMessage("File is too big")
					.Must(f => _allowedClipContentTypes.Contains(f!.ContentType)).WithMessage("Invalid video clip file format");
			});
		}
	}
}
