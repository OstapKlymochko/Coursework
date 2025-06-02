using Common.Services.Interfaces;
using MusicService.Models;
using MusicService.Services.Interfaces;

namespace MusicService.Services
{
    public class CollectionsDbService : ICollectionsDbService
	{
		private readonly IDataAccessService _dataAccessService;
		public CollectionsDbService(IDataAccessService dataAccessService)
		{
			_dataAccessService = dataAccessService;
		}

		public Task CreateCollectionAsync(CollectionDbModel collection)
		{
			string query = "insert into collections (ownerid, title, ownerusername, type, imgfilekey) VALUES (@OwnerId, @Title, @OwnerUsername, @Type, 'default_playlist.svg');";
			return _dataAccessService.ExecuteStatementAsync(query, collection);
		}

		public Task<CollectionDbModel?> GetCollectionByNameAsync(string name, int userId)
		{
			string query = "select * from collections where title = @Title and ownerid = @OwnerId";
			return _dataAccessService.QuerySingleRecordAsync<CollectionDbModel, dynamic>(query, new { OwnerId = userId, Title = name });
		}

		public Task<CollectionDbModel?> GetCollectionByIdAsync(int userId, int collectionId)
		{
			string query = "select * from collections where id = @Id and ownerid = @OwnerId";
			return _dataAccessService.QuerySingleRecordAsync<CollectionDbModel, dynamic>(query, new { OwnerId = userId, Id = collectionId });
		}

		public Task<IEnumerable<CollectionDto>> GetUserCollectionsAsync(int userId)
		{
			string query = "select c.imgfilekey as ImageUrl, c.id, c.title, c.ownerusername as OwnerUserName, c.ownerid, c.type, count(sc.songid) as SongsCount "
                + "from collections c left join song_collections sc on c.ownerid = sc.ownerid and c.id = sc.collectionid "
				+ "where c.ownerid = @OwnerId "
				+ "group by c.ownerid, sc.collectionid, c.title, c.id, c.ownerusername, c.type, c.imgfilekey;";
			return _dataAccessService.QueryDataAsync<CollectionDto, dynamic>(query, new { OwnerId = userId });
		}

		public async Task<bool> IsSongInCollection(int songId, int ownerId, int collectionId)
		{
			string query = "select 1 from song_collections where collectionid = @Id and ownerid = @OwnerId and songid = @SongId";
			var result = await _dataAccessService.QuerySingleRecordAsync<string, dynamic>(query, new { Id = collectionId, OwnerId = ownerId, SongId = songId });
			return result == "1";
		}
		public Task AddSongToCollectionAsync(int songId, int ownerId, int collectionId)
		{
			string query = "insert into song_collections (songid, ownerid, collectionid) VALUES (@SongId, @OwnerId, @CollectionId)";
			return _dataAccessService.ExecuteStatementAsync<dynamic>(query, new { SongId = songId, OwnerId = ownerId, CollectionId = collectionId });
		}
		public Task RemoveSongFromCollectionAsync(int songId, int ownerId, int collectionId)
		{
			string query = "delete from song_collections where songid = @SongId and collectionid = @CollectionId and ownerid = @OwnerId";
			return _dataAccessService.ExecuteStatementAsync<dynamic>(query, new { SongId = songId, OwnerId = ownerId, CollectionId = collectionId });
		}
		public Task<IEnumerable<SongDto>> GetCollectionSongsAsync(int ownerId, int collectionId)
		{
			string query = "select s.id, s.title, g.title as Genre, logofilekey as LogoUrl, s.authorId, s.author_pseudonym as AuthorPseudonym "
				+ "from song_collections sc join songs s on s.id = sc.songid "
				+ "join genres g on s.genreid = g.id "
				+ "where collectionid = @CollectionId and ownerid = @OwnerId";
			return _dataAccessService.QueryDataAsync<SongDto, dynamic>(query, new { OwnerId = ownerId, CollectionId = collectionId });
		}
		public Task DeleteCollectionAsync(int collectionId, int userId)
		{
			string query = "delete from collections where id = @CollectionId and ownerid = @OwnerId";
			return _dataAccessService.ExecuteStatementAsync<dynamic>(query, new { CollectionId = collectionId, OwnerId = userId });
		}
	}
}
