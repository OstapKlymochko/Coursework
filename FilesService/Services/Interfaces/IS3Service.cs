using Amazon.S3.Model;

namespace FilesService.Services.Interfaces
{
	public interface IS3Service
	{
		public Task<GetObjectResponse?> GetFileAsync(string fileName);
		public Task UploadFileAsync(IFormFile file, string fileName);
		public Task<string?> GetPreSignedUrl(string fileKey);
	}
}
