using Common.CommonTypes.Interfaces;
using Common.Services.Interfaces;
using MusicService.Models;
using MusicService.Services.Interfaces;

namespace MusicService.Services
{
    public class SongsDbService : ISongsDbService
    {
        private readonly IDataAccessService _dataAccessService;
        private readonly IGenericRepository<SongDbModel> _songRepository;

        public SongsDbService(IDataAccessService dataAccessService, IGenericRepository<SongDbModel> songRepository)
        {
            _dataAccessService = dataAccessService;
            _songRepository = songRepository;
        }

        public Task<SongDetailsDto?> GetSongByIdAsync(int id, int userId)
        {
            string query = "select s.*, s.logofilekey as logourl, s.videoclipfilekey as VideoClipUrl, s.songfilekey as songurl, s.author_pseudonym as AuthorPseudonym,"
                + " COUNT(CASE WHEN sr.type = 'like' THEN 1 END) AS Likes, COUNT(CASE WHEN sr.type = 'dislike' THEN 1 END) AS Dislikes,"
                + " MAX(CASE WHEN sr.userid = @UserId AND sr.type = 'like' THEN 1 ELSE 0 END) AS isLiked, MAX(CASE WHEN sr.userid = @UserId AND sr.type = 'dislike' THEN 1 ELSE 0 END) AS isDisliked"
                + " from songs s LEFT JOIN song_reactions sr ON s.id = sr.songid"
                + " where s.id = @Id group by s.id;";
            return _dataAccessService.QuerySingleRecordAsync<SongDetailsDto, dynamic>(query, new { Id = id, UserId = userId });
        }
        //todo add filters
        public Task<IEnumerable<SongDto>> GetSongListPaginatedAsync(int select = 10, int skip = 0, string key = "")
        {
            string query = "select s.id, s.title, g.title as Genre, logofilekey as LogoUrl, s.authorId, s.author_pseudonym as AuthorPseudonym ";
            if (key != string.Empty) query += ", ts_rank(ts, to_tsquery('english', @Key)) as rank ";
            query += "from songs s left join genres g on g.id = s.genreid ";
            if (key != string.Empty) query += "order by rank desc ";
            query += "limit @Select offset @Skip;";
            return _dataAccessService.QueryDataAsync<SongDto, dynamic>(query, new { Select = select, Skip = skip, Key = "%" + key + "%" });
        }

        public Task<IEnumerable<string>> GetSongNamesByKeyAsync(string key, int select = 10)
        {
            //full text search postgres
            //string query = "select title from songs group by title, author_pseudonym "
            //    + "having title ilike @Key or author_pseudonym ilike @Key "
            //+ "order by count(title) desc limit @Limit;";
            string query = "with t as (select title, ts_rank(ts, to_tsquery('english', @Key)) as r"
                + " from songs order by r desc limit @Limit)"
                + " select * from t where r > 0;";
            return _dataAccessService.QueryDataAsync<string, dynamic>(query, new { Key = key, Limit = select });
        }

        public Task SaveSongAsync(SongDbModel song) => _songRepository.CreateAsync(song, true);

        public Task<SongStatisticsData?> GetSongStatisticsAsync(int songId, int userId)
        {
            string query = " select count(case when sr.type = 'like' then 1 end) as Likes,"
                + " count(case when sr.type = 'dislike' then 1 end) as Dislikes,"
                + " (case when sr1.type = 'like' then true else false end) as isLiked,"
                + " (case when sr1.type = 'dislike' then true else false end) as isDisliked"
                + " from songs s left join song_reactions sr on s.id = sr.songid left join song_reactions sr1 on s.id = sr1.songid and sr1.userid = @UserId where s.id = @SongId"
                + " group by sr.songid, s.songfilekey, s.videoclipfilekey, sr1.type;";
            return _dataAccessService.QuerySingleRecordAsync<SongStatisticsData, dynamic>(query, new { SongId = songId, UserId = userId });
        }

        public Task<SongsCountModel> GetSongsCount(string key = "")
        {
            var query = "select count(*) as count from songs ";
            if (key != string.Empty) query += "where title like @Key or author_pseudonym like @Key;";
            return _dataAccessService.QuerySingleRecordAsync<SongsCountModel, dynamic>(query, new { Key = "%" + key + "%" })!;
        }

        public Task RegisterSongListened(SongDbListeningModel listeningModel)
        {
            string query = "insert into song_listening (timestamp, userId, songId) VALUES (@Timestamp, @UserId, @SongId);";
            return _dataAccessService.ExecuteStatementAsync<dynamic>(query, new { Timestamp = listeningModel.ListenedAt, UserId = listeningModel.UserId, SongId = listeningModel.UserId });
        }
    }
}
