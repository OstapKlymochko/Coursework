namespace MusicService.Services.Interfaces
{
    public interface IFilesService
    {
        public Task<string> GetFileUrlByKeyAsync(string key);
        public Task<string> GenerateThumbnailAndSaveAsync(IFormFile file, int width, int height);
        public Task<string> SaveFileAsync(IFormFile file);
    }
}
