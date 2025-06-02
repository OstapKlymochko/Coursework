using Microsoft.AspNetCore.Mvc;
using StatisticsService.Models;
using StatisticsService.Services.Interfaces;
using StatisticsService.Helpers;

namespace StatisticsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IReactionsService _reactionsService;
        private readonly ICommentsService _commentsService;

        public StatisticsController(IReactionsService reactionsService, ICommentsService commentsService)
        {
            _reactionsService = reactionsService;
            _commentsService = commentsService;
        }

        [HttpPost("reaction")]
        public async Task<IActionResult> AddReaction(CreateReactionModel reaction)
        {
            int userId = this.ExtractIdFromToken();
            var result = await _reactionsService.CreateReactionAsync(reaction, userId);
            if (result.IsFailure) return result.MapToResponse();
            return Ok();
        }

        [HttpDelete("reaction/{songId:int}")]
        public async Task<IActionResult> DeleteReaction([FromRoute] int songId)
        {
            if (songId <= 0) return BadRequest();
            int userId = this.ExtractIdFromToken();
            await _reactionsService.RemoveReactionAsync(userId, songId);
            return Ok();
        }
        //?????????????? statistics or songs
        [HttpGet("comments/{songId:int}")]
        public async Task<IActionResult> GetCommentsPaginated([FromRoute] int songId, [FromQuery] int select = 10,
            [FromQuery] int skip = 0, [FromQuery] int? parentCommentId = null)
        {
            if (songId <= 0) return BadRequest();
            var comments = await _commentsService.GetSongCommentsPaginated(songId, select, skip, parentCommentId);
            return Ok(comments);
        }

        [HttpPost("comments")]
        public async Task<IActionResult> CreateComment(CreateCommentModel comment)
        {
            int userId = this.ExtractIdFromToken();
            var result = await _commentsService.CreateCommentAsync(comment, userId);
            return result.MapToResponse();
        }
        [HttpPut("comments")]
        public async Task<IActionResult> UpdateComment(UpdateCommentModel comment)
        {
            int userId = this.ExtractIdFromToken();
            var result = await _commentsService.UpdateCommentAsync(comment, userId);
            if (result.IsSuccess) return Ok();
            return result.MapToResponse();
        }

        [HttpDelete("comments/{commentId:int}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int commentId)
        {
            await _commentsService.DeleteCommentAsync(commentId);
            return Ok();
        }
    }
}
