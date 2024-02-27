using Common.CommonTypes.Interfaces;
using Common.Services;
using FilesService.Models;
using FilesService.Services.Interfaces;

namespace FilesService.Services
{
	public class FileDbService : IFileDbService
	{
		private readonly IGenericRepository<FileDbModel> _fileRepository;
		private readonly IDataAccessService _accessService;

		public FileDbService(IGenericRepository<FileDbModel> fileRepository, IDataAccessService accessService)
		{
			_fileRepository = fileRepository;
			_accessService = accessService;
		}

		public Task<FileDbModel?> GetFileByIdAsync(int id) => _fileRepository.GetByIdAsync(id);


		public Task<FileDbModel?> GetFileByNameAsync(string fileName) => _accessService
			.QuerySingleRecordAsync<FileDbModel, dynamic>("select * from files where displayFileName = @Name", new { Name = fileName });


		public Task<IEnumerable<FileDto>> GetFilesByNameAsync(string fileName)
		{
			return _accessService
				.QueryDataAsync<FileDto, dynamic>("select id, displayFileName from files where displayFileName ilike @Name", new { Name = "%" + fileName + "%"});
		}

		public async Task CreateFile(FileDbModel fileModel)
		{
			await _fileRepository.CreateAsync(fileModel, true);
		}

	}
}
