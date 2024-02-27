namespace FilesService.Models
{
	public class UploadFileModel
	{
		public string DisplayName { get; set; } = null!;
		public IFormFile File { get; set; } = null!;
	}
}
