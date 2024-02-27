using Common.CommonTypes;
using Common.CommonTypes.Interfaces;
using Common.Contracts;
using Common.Errors;
using Common.Result;
using FilesService.Models;
using FilesService.Services.Interfaces;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace FilesService.Services
{
	public class FilesService : IFilesService
	{
		private readonly IS3Service _s3Service;
		private readonly IFileDbService _fileDbService;
		private readonly IRequestClient<CheckUserRoleContract> _client;
		private readonly IValidator<UploadFileModel> _uploadFileValidator;

		public FilesService(IGenericRepository<FileDbModel> fileDbRepository, IS3Service s3Service, IFileDbService fileDbService, IRequestClient<CheckUserRoleContract> client, IValidator<UploadFileModel> uploadFileValidator)
		{
			_s3Service = s3Service;
			_fileDbService = fileDbService;
			_client = client;
			_uploadFileValidator = uploadFileValidator;
		}

		public async Task<ServiceResult<FileStreamResult>> GetFileById(int id)
		{
			FileDbModel? file = await _fileDbService.GetFileByIdAsync(id);
			if (file == null) return new NotFoundError("File not found");
			var response = await _s3Service.GetFileAsync(file.FileName);
			if (response == null) return new NotFoundError("File not found");

			return new FileStreamResult(response.ResponseStream, response.Headers.ContentType);
		}

		public async Task<ServiceResult<FileStreamResult>> GetFileByNameAsync(string fileName)
		{
			FileDbModel? file = await _fileDbService.GetFileByNameAsync(fileName);

			if (file == null) return new NotFoundError("File not found");
			var response = await _s3Service.GetFileAsync(file.FileName);
			if (response == null) return new NotFoundError("File not found");

			return new FileStreamResult(response.ResponseStream, response.Headers.ContentType);
		}

		public async Task<ServiceResult<FileUrlModel>> GetPreSignedUrl(int id)
		{
			FileDbModel? file = await _fileDbService.GetFileByIdAsync(id);
			if (file == null) return new NotFoundError("File not found");
			var url = await _s3Service.GetPreSignedUrl(file.FileName);
			if(url == null) return new NotFoundError("File not found");
			return new FileUrlModel { Url = url };
		}

		public async Task<ServiceResult<FileListModel>> GetFilesByNameAsync(string fileName)
		{
			if (string.IsNullOrEmpty(fileName)) return new ModelError("Invalid name");
			var files = await _fileDbService.GetFilesByNameAsync(fileName);
			return new FileListModel { Result = files };
		}

		public async Task<ServiceResult<BasicResponse>> UploadFileAsync(UploadFileModel uploadModel, int userId)
		{
			var isAuthor = await CheckUserRole(userId);
			if (!isAuthor) return new ModelError("User is not allowed to perform this operation or does not exist");

			var validationResult = await _uploadFileValidator.ValidateAsync(uploadModel);
			if (!validationResult.IsValid) return new ModelError(string.Join(", ", validationResult.Errors));
			var contentType = uploadModel.File.ContentType.Split("/");

			string fileName = Guid.NewGuid() + $".{contentType[1]}";
			var fileDbModel = new FileDbModel()
			{
				DisplayFileName = uploadModel.DisplayName,
				FileName = fileName,
				FileType = DefineFileType(contentType[0])
			};
			await _s3Service.UploadFileAsync(uploadModel.File, fileName);
			await _fileDbService.CreateFile(fileDbModel);
			return new BasicResponse("File was successfully uploaded");
		}

		private async Task<bool> CheckUserRole(int userId)
		{
			var response = await _client.GetResponse<CheckUserRoleResponse, NotFoundError>(new CheckUserRoleContract() { UserId = userId });
			if (response.Is(out Response<NotFoundError>? notFoundResponse)) return false;
			if (response.Is(out Response<CheckUserRoleResponse>? checkUserRoleResponse))
				return checkUserRoleResponse.Message.IsAuthor;
			return false;
		}

		private string DefineFileType(string mimeType)
		{
			var types = new Dictionary<string, string>()
			{
				{"audio", "song"},
				{"video", "video-clip"},
				{"image", "logo"}
			};
			return types[mimeType];
		}
		public int ExtractIdFromToken(HttpContext ctx)
		{
			return int.Parse(ctx.User.Claims.First(c => c.Type == "userId").Value);
		}
	}
}
