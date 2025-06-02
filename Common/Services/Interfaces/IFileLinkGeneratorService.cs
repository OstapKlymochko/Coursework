namespace Common.Services.Interfaces
{
    public interface IFileLinkGeneratorService
    {
        public Task<string?> GetPreSignedUrl(string fileKey);
    }
}
