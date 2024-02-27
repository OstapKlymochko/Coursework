using Common.CommonTypes;
using FilesService.Helpers;
using FilesService.Models;
using FilesService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FilesService.Controllers
{
	[ApiController]
	[Route("/api/[controller]")]
	public class FilesController : ControllerBase
	{
		private readonly IFilesService _filesService;
		public FilesController(IFilesService filesService)
		{
			_filesService = filesService;
		}
		[HttpGet("get/{fileId:int}")]
		public async Task<IActionResult> GetFile([FromRoute] int fileId)
		{
			if (fileId == default) return new BadRequestObjectResult(new BasicResponse("Invalid parameter value"));
			var result = await _filesService.GetFileById(fileId);
			if (result.Value is FileStreamResult) return result.Value;
			return result.MapToResponse();
		}

		[HttpGet("get/{fileName}")]
		public async Task<IActionResult> GetFilesByName([FromRoute] string fileName)
		{
			var result = await _filesService.GetFilesByNameAsync(fileName);
			return result.MapToResponse();
		}

		[HttpGet("get-url/{fileId:int}")]
		public async Task<IActionResult> GetFileUrl([FromRoute] int fileId)
		{
			if (fileId == default) return new BadRequestObjectResult(new BasicResponse("Invalid parameter value"));
			var result = await _filesService.GetPreSignedUrl(fileId);
			return result.MapToResponse();
		}

		[HttpPost("upload")]
		public async Task<IActionResult> GetFilesList([FromForm] UploadFileModel model)
		{
			int userId = _filesService.ExtractIdFromToken(HttpContext);
			var result = await _filesService.UploadFileAsync(model, userId);
			return result.MapToResponse();
		}

	}
}