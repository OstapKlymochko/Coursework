using MusicService.Helpers;
using MusicService.Models;
using MusicService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MusicService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly ISongsService _songsService;
        private readonly ICollectionsService _collectionsService;
        private readonly IReactionsService _reactionsService;
        private readonly ICommentsService _commentsService;

        public SongsController(ISongsService songsService, ICollectionsService collectionsService, ICommentsService commentsService, IReactionsService reactionsService)
        {
            _songsService = songsService;
            _collectionsService = collectionsService;
            _commentsService = commentsService;
            _reactionsService = reactionsService;
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetSongsList([FromQuery] uint select, [FromQuery] uint skip, [FromQuery] string key = "")
        {
            if (select <= 0) return BadRequest();
            var result = await _songsService.GetSongsPaginatedAsync(select, skip, key);
            return result.MapToResponse();
        }

        [HttpGet("get/{songId:int}")]
        public async Task<IActionResult> GetSongById([FromRoute] int songId)
        {
            int userId = this.ExtractIdFromToken();
            var result = await _songsService.GetSongById(songId, userId);
            return result.MapToResponse();
        }

        [HttpGet("get/{songId:int}/statistics")]
        public async Task<IActionResult> GetSongStatisticsById([FromRoute] int songId)
        {
            int userId = this.ExtractIdFromToken();
            var result = await _songsService.GetSongStatisticsAsync(songId, userId);
            return result.MapToResponse();
        }

        [HttpGet("get-suggestions")]
        public async Task<IActionResult> GetSongNamesByKey([FromQuery] string key)
        {
            var result = await _songsService.GetSongNamesByKey(key);
            return result.MapToResponse();
        }

        [HttpPost("create")]
        [Authorize(Roles = "Author")]
        public async Task<IActionResult> UploadSong([FromForm] UploadSongModel uploadSongModel)
        {
            var userId = this.ExtractIdFromToken();
            var result = await _songsService.UploadSongAsync(uploadSongModel, userId);
            return result.MapToResponse();
        }

        [HttpGet("genres")]
        public async Task<IActionResult> GetGenres()
        {
            var genres = await _songsService.GetGenres();
            return Ok(genres);
        }

        [HttpPost("collections")]
        public async Task<IActionResult> CreateCollectionAsync(CreateCollectionModel collection)
        {
            if (!this.User.IsInRole("Author") && collection.Type == "Album") return Forbid();
            int userId = this.ExtractIdFromToken();
            var result = await _collectionsService.CreateCollection(collection, userId);
            return result.MapToResponse();
        }

        [HttpDelete("collections/{collectionId:int}")]
        public async Task<IActionResult> DeleteCollectionAsync([FromRoute] int collectionId)
        {
            if (collectionId <= 0) return BadRequest();
            int userId = this.ExtractIdFromToken();
            var result = await _collectionsService.RemoveCollectionAsync(userId, collectionId);
            return result.MapToResponse();
        }

        [HttpGet("collections")]
        public async Task<IActionResult> GetUsersCollections()
        {
            int userId = this.ExtractIdFromToken();
            var result = await _collectionsService.GetUsersCollectionsAsync(userId);
            return result.MapToResponse();
        }

        [HttpGet("collections/{collectionId:int}")]
        public async Task<IActionResult> GetCollectionsSongs([FromRoute] int collectionId)
        {
            int userId = this.ExtractIdFromToken();
            var result = await _collectionsService.GetCollectionSongsAsync(userId, collectionId);
            return result.MapToResponse();
        }

        [HttpPut("collections/add-song")]
        public async Task<IActionResult> AddSongToCollectionAsync(CollectionSongListUpdateModel addSongModel)
        {
            if (addSongModel.SongId <= 0 || addSongModel.CollectionId < 0) return BadRequest();
            int userId = this.ExtractIdFromToken();
            bool isAuthor = User.IsInRole("Author");
            var result = await _collectionsService.AddSongToCollectionAsync(userId, addSongModel.CollectionId, addSongModel.SongId, isAuthor);
            return result.MapToResponse();
        }

        [HttpPut("collections/remove-song")]
        public async Task<IActionResult> RemoveSongFromCollectionAsync(CollectionSongListUpdateModel addSongModel)
        {
            if (addSongModel.SongId <= 0 || addSongModel.CollectionId <= 0) return BadRequest();
            int userId = this.ExtractIdFromToken();
            bool isAuthor = User.IsInRole("Author");
            var result = await _collectionsService.RemoveSongFromCollectionAsync(userId, addSongModel.CollectionId, addSongModel.SongId, isAuthor);
            return result.MapToResponse();
        }
        //from statistics service
        [HttpPost("statistics/reaction")]
        public async Task<IActionResult> AddReaction(CreateReactionModel reaction)
        {
            int userId = this.ExtractIdFromToken();
            var result = await _reactionsService.CreateReactionAsync(reaction, userId);
            if (result.IsFailure) return result.MapToResponse();
            return Ok();
        }

        [HttpDelete("statistics/reaction/{songId:int}")]
        public async Task<IActionResult> DeleteReaction([FromRoute] int songId)
        {
            if (songId <= 0) return BadRequest();
            int userId = this.ExtractIdFromToken();
            await _reactionsService.RemoveReactionAsync(userId, songId);
            return Ok();
        }

        [HttpGet("statistics/comments/{songId:int}")]
        public async Task<IActionResult> GetCommentsPaginated([FromRoute] int songId, [FromQuery] int select = 10,
            [FromQuery] int skip = 0, [FromQuery] int? parentCommentId = null)
        {
            if (songId <= 0) return BadRequest();
            var comments = await _commentsService.GetSongCommentsPaginated(songId, select, skip, parentCommentId);
            return Ok(comments);
        }

        [HttpPost("statistics/comments")]
        public async Task<IActionResult> CreateComment(CreateCommentModel comment)
        {
            int userId = this.ExtractIdFromToken();
            var result = await _commentsService.CreateCommentAsync(comment, userId);
            return result.MapToResponse();
        }
        [HttpPut("statistics/comments")]
        public async Task<IActionResult> UpdateComment(UpdateCommentModel comment)
        {
            int userId = this.ExtractIdFromToken();
            var result = await _commentsService.UpdateCommentAsync(comment, userId);
            if (result.IsSuccess) return Ok();
            return result.MapToResponse();
        }

        [HttpDelete("statistics/comments/{commentId:int}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int commentId)
        {
            await _commentsService.DeleteCommentAsync(commentId);
            return Ok();
        }

        [HttpPost("statistics/views/{songId:int}")]
        public async Task<IActionResult> RegisterListening(SongPlayDto songInteractionDto, [FromRoute] int songId)
        {
            if (songId <= 0) return BadRequest();

            int userId = this.ExtractIdFromToken();
            songInteractionDto.SongId = songId;
            songInteractionDto.UserId = userId;
            var result = await _songsService.RegisterSongListened(songInteractionDto);

            if (result.IsSuccess) return Ok();
            else return result.MapToResponse();
        }
    }
}
