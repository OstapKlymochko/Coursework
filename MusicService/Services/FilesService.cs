using MusicService.Services.Interfaces;

namespace MusicService.Services
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

        public Task<string> GetFileUrlByKeyAsync(string key) => _s3Service.GetPreSignedUrl(key)!;

        public async Task<string> GenerateThumbnailAndSaveAsync(IFormFile file, int width, int height)
        {
            try
            {
                using var stream = file.OpenReadStream();
                var image = NetVips.Image.NewFromStream(stream);

                image = image.ThumbnailImage(width, height, size: NetVips.Enums.Size.Both);
                var imageBytes = image.WriteToMemory();

                string contentType = GetContentType(file);
                string fileName = Guid.NewGuid() + $".{contentType}";
                return fileName;

            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return string.Empty;
            }
        }

        private string GetContentType(IFormFile file) => file.ContentType.Split("/")[1];

    }
}
