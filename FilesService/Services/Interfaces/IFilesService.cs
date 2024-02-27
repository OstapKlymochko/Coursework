using Common.CommonTypes;
using Common.Result;
using FilesService.Models;
using Microsoft.AspNetCore.Mvc;

namespace FilesService.Services.Interfaces
{
	public interface IFilesService
	{
		public Task<ServiceResult<FileStreamResult>> GetFileById(int id);
		public Task<ServiceResult<FileStreamResult>> GetFileByNameAsync(string fileName);
		public Task<ServiceResult<FileListModel>> GetFilesByNameAsync(string fileName);
		public Task<ServiceResult<BasicResponse>> UploadFileAsync(UploadFileModel uploadModel, int userId);
		public Task<ServiceResult<FileUrlModel>> GetPreSignedUrl(int id);
		public int ExtractIdFromToken(HttpContext ctx);
	}
}
