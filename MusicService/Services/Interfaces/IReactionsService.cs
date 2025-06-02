using Common.Result;
using MusicService.Models;

namespace MusicService.Services.Interfaces
{
	public interface IReactionsService
	{
		public Task<ServiceResult<int>> CreateReactionAsync(CreateReactionModel reaction, int userId);
		public Task RemoveReactionAsync(int userId, int songId);
	}
}
