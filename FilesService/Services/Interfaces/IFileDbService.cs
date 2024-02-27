using FilesService.Models;

namespace FilesService.Services.Interfaces
{
	public interface IFileDbService
	{
		public Task<FileDbModel?> GetFileByIdAsync(int id);
		public Task<FileDbModel?> GetFileByNameAsync(string fileName);
		public Task<IEnumerable<FileDto>> GetFilesByNameAsync(string fileName);
		public Task CreateFile(FileDbModel fileModel);
	}
}
