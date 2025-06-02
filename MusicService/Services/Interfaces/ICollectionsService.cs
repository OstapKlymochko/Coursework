using Common.CommonTypes;
using Common.Result;
using MusicService.Models;

namespace MusicService.Services.Interfaces
{
	public interface ICollectionsService
	{
		public Task<ServiceResult<BasicResponse>> CreateCollection(CreateCollectionModel collection, int userId);
		public Task<ServiceResult<CollectionsListDto>> GetUsersCollectionsAsync(int userId);
		public Task<ServiceResult<BasicResponse>> AddSongToCollectionAsync(int userId, int collectionId, int songId, bool isAuthor);
		public Task<ServiceResult<BasicResponse>> RemoveSongFromCollectionAsync(int userId, int collectionId, int songId, bool isAuthor);
		public Task<ServiceResult<SongsListDto>> GetCollectionSongsAsync(int userId, int collectionId);
		public Task<ServiceResult<BasicResponse>> RemoveCollectionAsync(int userId, int collectionId);
	}
}
