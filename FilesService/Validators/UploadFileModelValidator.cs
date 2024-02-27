using FilesService.Models;
using FluentValidation;

namespace FilesService.Validators
{
	public class UploadFileModelValidator: AbstractValidator<UploadFileModel>
	{
		public UploadFileModelValidator()
		{
			List<string> allowedContentTypes = new()
			{
				"image/jpeg",
				"image/png",
				"audio/mp3",
				"video/mp4"
			};
			RuleFor(m => m.File).NotNull().NotEmpty().WithMessage("File must not be empty")
				.Must(f => f.Length <= 100 * 1024 * 1024)
				.Must(f => allowedContentTypes.Contains(f.ContentType));
			RuleFor(m => m.DisplayName).NotNull().NotEmpty().WithMessage("Name must not be empty");
		}
	}
}
