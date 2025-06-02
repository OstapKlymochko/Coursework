using FilesService.Services.Interface;

namespace FilesService.Services
{
    public class FilesService : IFilesService
    {
        private readonly IS3Service _s3Service;

        public FilesService(IS3Service s3Service)
        {
            _s3Service = s3Service;
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            string contentType = GetContentType(file);
            string fileName = Guid.NewGuid() + $".{contentType}";
            await _s3Service.UploadFileAsync(file, fileName);
            return fileName;
        }

        public Task DeleteFileAsync(string fileKey) => _s3Service.DeleteFileAsync(fileKey);
        public Task<string> GetFileUrlByKeyAsync(string key) => _s3Service.GetPreSignedUrl(key)!;
        private string GetContentType(IFormFile file) => file.ContentType.Split("/")[1];
    }
}
