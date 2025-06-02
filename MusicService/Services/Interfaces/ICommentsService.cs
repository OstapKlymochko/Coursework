using Common.Result;
using MusicService.Models;

namespace MusicService.Services.Interfaces
{
    public interface ICommentsService
    {
        public Task<int> GetCommentsCountAsync(int songId);
        public Task<CommentsListModel> GetSongCommentsPaginated(int songId, int select = 10, int skip = 0, int? parrentCommentId = null);
        public Task<ServiceResult<CommentDto>> CreateCommentAsync(CreateCommentModel comment, int userId);
        public Task<ServiceResult<int>> UpdateCommentAsync(UpdateCommentModel comment, int userId);
        public Task DeleteCommentAsync(int commentId);
    }
}
