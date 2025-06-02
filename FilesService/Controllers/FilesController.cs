using Common.CommonTypes;
using Common.Contracts;
using Common.Services.Interfaces;
using FilesService.Helpers;
using FilesService.Models;
using FilesService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilesService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : Controller
    {
        private readonly IMessageQueueService _messageQueueService;
        private readonly IFilesService _filesService;
        public FilesController(IMessageQueueService messageQueueService, IFilesService filesService)
        {
            _messageQueueService = messageQueueService;
            _filesService = filesService;
        }

        [HttpPost("song")]
        [Authorize(Roles = "Author")]
        public async Task<IActionResult> CreateSongAsync([FromForm] UploadSongModel uploadSongModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.GetModelErrors());

            var songCreatedMessage = new SongUploadedContract()
            {
                AuthorId = this.ExtractIdFromToken(),
                AuthorPseudonym = uploadSongModel.Pseudonym,
                CreatedAt = DateTime.UtcNow,
                GenreId = uploadSongModel.GenreId,
                Title = uploadSongModel.Title
            };

            if (uploadSongModel.Logo != null) songCreatedMessage.LogoFileKey = await _filesService.SaveFileAsync(uploadSongModel.Logo);
            if (uploadSongModel.VideoClip != null) songCreatedMessage.VideoClipFileKey = await _filesService.SaveFileAsync(uploadSongModel.VideoClip);
            songCreatedMessage.SongFileKey = await _filesService.SaveFileAsync(uploadSongModel.Song);

            await _messageQueueService.PublishMessageAsync(songCreatedMessage);
            return Ok(new BasicResponse("Uploaded successfully"));
        }

        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatarAsync([FromForm] UploadAvatarModel avatarModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.GetModelErrors());
            var fileKey = await _filesService.SaveFileAsync(avatarModel.Avatar);

            int userId = this.ExtractIdFromToken();
            var message = new AvatarUploadedContract()
            {
                FileKey = fileKey,
                UserId = userId
            };
            await _messageQueueService.PublishMessageAsync(message);

            return Ok(new BasicResponse("Uploaded successfully"));
        }
    }
}
