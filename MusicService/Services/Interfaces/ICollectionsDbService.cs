using MusicService.Models;

namespace MusicService.Services.Interfaces
{
	public interface ICollectionsDbService
	{
		public Task CreateCollectionAsync(CollectionDbModel collection);
		public Task<IEnumerable<CollectionDto>> GetUserCollectionsAsync(int userId);
		public Task<CollectionDbModel?> GetCollectionByNameAsync(string name, int userId);
		public Task<CollectionDbModel?> GetCollectionByIdAsync(int userId, int collectionId);
		public Task AddSongToCollectionAsync(int songId, int ownerId, int collectionId);
		public Task RemoveSongFromCollectionAsync(int songId, int ownerId, int collectionId);
		public Task<IEnumerable<SongDto>> GetCollectionSongsAsync(int ownerId, int collectionId);
		public Task DeleteCollectionAsync(int collectionId, int userId);
		public Task<bool> IsSongInCollection(int songId, int ownerId, int collectionId);
	}
}
