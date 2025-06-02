using StatisticsService.Models;

namespace StatisticsService.Services.Interfaces
{
    public interface ICommentsDbService
    {
        public Task<CommentDbModel?> GetCommentByIdAsync(int commentId);
        public Task<string?> GetCommentsCountAsync(int songId);
        public Task<IEnumerable<CommentDto>> GetSongCommentsPaginatedAsync(int songId, int select = 10, int skip = 0);
        public Task<IEnumerable<CommentDto>> GetCommentRepliesPaginatedAsync(int commentId, int select = 10, int skip = 0);
        public Task<string?> GetCommentsCountAsync(int songId, int? parentCommentId = null);
        public Task<CommentDto> CreateCommentAsync(CommentDbModel comment);
        public Task UpdateCommentBodyAsync(UpdateCommentModel comment);
        public Task SetIsCommentAttachedAsync(int commentId, bool attached);
        public Task<string> DeleteCommentAsync(int commentId);
    }
}
