using Common.Services.Interfaces;
using MusicService.Models;
using MusicService.Services.Interfaces;

namespace MusicService.Services
{
    public class CommentsDbService : ICommentsDbService
    {
        private readonly IDataAccessService _dataAccessService;

        public CommentsDbService(IDataAccessService dataAccessService)
        {
            _dataAccessService = dataAccessService;
        }

        public Task<string?> GetCommentsCountAsync(int songId)
        {
            string query = "select count(*) from comments where songid = @SongId";
            return _dataAccessService.QuerySingleRecordAsync<string, dynamic>(query, new { @SongId = songId });
        }

        public Task<IEnumerable<CommentDto>> GetSongCommentsPaginatedAsync(int songId, int select = 10, int skip = 0)
        {
            string query = "select c1.*, count(c2.repliedTo) as RepliesCount from comments c1 "
                + "left join comments c2 on c1.id = c2.repliedTo where c1.repliedTo is null and c1.songId = @SongId "
                + "group by c1.id, c1.userid, c1.username, c1.createdAt, c1.body, c1.edited, c1.attached, c1.songId "
                + "order by c1.createdAt desc limit @Limit offset @Offset;";
            return _dataAccessService.QueryDataAsync<CommentDto, dynamic>(query, new { SongId = songId, Limit = select, Offset = skip });
        }
        public Task<CommentDbModel?> GetCommentByIdAsync(int commentId)
        {
            string query = "select * from comments where id = @CommentId order by createdAt;";
            return _dataAccessService.QuerySingleRecordAsync<CommentDbModel, dynamic>(query, new { CommentId = commentId });
        }
        public Task<IEnumerable<CommentDto>> GetCommentRepliesPaginatedAsync(int commentId, int select = 10, int skip = 0)
        {
            string query = "select * from comments where repliedTo = @CommentId order by createdAt desc limit @Limit offset @Offset;";
            return _dataAccessService.QueryDataAsync<CommentDto, dynamic>(query, new { CommentId = commentId, Limit = select, Offset = skip });
        }

        public Task<CommentDto> CreateCommentAsync(CommentDbModel comment)
        {
            string query = "insert into comments (userId, username, body, songId, createdat, attached, repliedto) values (@UserId, @UserName, @Body, @SongId, @CreatedAt, @Attached, @RepliedTo) returning *;";
            return _dataAccessService.ExecuteStatementAndReturnAsync<CommentDto, dynamic>(query, comment);
        }

        public Task UpdateCommentBodyAsync(UpdateCommentModel comment)
        {
            string query = "update comments set body = @Body, edited = true where id = @CommentId";
            return _dataAccessService.ExecuteStatementAsync(query, comment);
        }

        public Task<string> DeleteCommentAsync(int commentId)
        {
            string query = "delete from comments where id = @Id returning songid";
            return _dataAccessService.ExecuteStatementAndReturnAsync<string, dynamic>(query, new { Id = commentId });
        }
        public Task SetIsCommentAttachedAsync(int commentId, bool attached)
        {
            string query = "update comments set attached = @Attached where id = @Id";
            return _dataAccessService.ExecuteStatementAsync(query, new { Id = commentId, Attached = attached });
        }

        public Task<string?> GetCommentsCountAsync(int songId, int? parentCommentId = null)
        {
            var query = "select count(*) from comments where songId = @SongId and repliedTo";
            if (parentCommentId != null) query += " = @RepliedTo";
            else query += " is null";
            query += ';';
            return _dataAccessService.QuerySingleRecordAsync<string, dynamic>(query, new { SongId = songId, RepliedTo = parentCommentId });
        }

    }
}
