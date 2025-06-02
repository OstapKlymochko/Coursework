namespace FilesService.Services.Interface
{
    public interface IFilesService
    {
        public Task<string> GetFileUrlByKeyAsync(string key);
        public Task<string> SaveFileAsync(IFormFile file);
        public Task DeleteFileAsync(string fileKey);
    }
}
