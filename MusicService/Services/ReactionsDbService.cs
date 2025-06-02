using Common.Services.Interfaces;
using MusicService.Models;
using MusicService.Services.Interfaces;

namespace MusicService.Services
{
    public class ReactionsDbService : IReactionsDbService
    {
        private readonly IDataAccessService _dataAccessService;
        public ReactionsDbService(IDataAccessService dataAccessService)
        {
            _dataAccessService = dataAccessService;
        }

        public Task<SongReactionsModel?> GetSongReactionsDataAsync(int songId)
        {
            string query = "select count(case when type = 'like' then 1 end) as Likes, "
                + "count(case when type = 'dislike' then 1 end) as Dislikes from song_reactions "
                + "where songId = @Id group by songId;";
            return _dataAccessService.QuerySingleRecordAsync<SongReactionsModel, dynamic>(query, new { Id = songId });
        }

        public Task<ReactionDbModel?> GetUserSongReactionAsync(int userId, int songId)
        {
            string query = "select * from song_reactions where userid = @UserId and songid = @SongId";
            return _dataAccessService.QuerySingleRecordAsync<ReactionDbModel, dynamic>(query, new { SongId = songId, UserId = userId });
        }

        public Task AddReactionAsync(CreateReactionModel reaction, int userId)
        {
            string query = "insert into song_reactions (userId, songId, type) values (@UserId, @SongId, @Type);";
            return _dataAccessService.ExecuteStatementAsync<dynamic>(query, new { UserId = userId, SongId = reaction.SongId, Type = reaction.Type });
        }
        public Task<ReactionDbModel> RemoveReactionAsync(int songId, int userId)
        {
            string query = "delete from song_reactions where userid = @UserId and songid = @SongId returning *;";
            return _dataAccessService.ExecuteStatementAndReturnAsync<ReactionDbModel, dynamic>(query, new { UserId = userId, SongId = songId });
        }

        public Task ToggleReactionAsync(int songId, int userId)
        {
            string query = "update song_reactions "
                + "set type = case when type = 'like' then 'dislike' else 'like' end "
                + "where userid = @UserId and songid = @SongId;";
            return _dataAccessService.ExecuteStatementAsync<dynamic>(query, new { UserId = userId, SongId = songId });
        }

        public async Task<bool> ReactionAlreadyExists(int songId, int userId)
        {
            string query = "select 1 from song_reactions where songid = @SongId and userid = @UserId;";
            var filters = new { UserId = userId, SongId = songId };
            var result = await _dataAccessService.QuerySingleRecordAsync<string, dynamic>(query, filters);
            return result == "1";
        }
    }
}
