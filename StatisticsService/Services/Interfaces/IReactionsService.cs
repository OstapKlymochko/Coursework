using Common.Result;
using StatisticsService.Models;

namespace StatisticsService.Services.Interfaces
{
	public interface IReactionsService
	{
		public Task<ServiceResult<int>> CreateReactionAsync(CreateReactionModel reaction, int userId);
		public Task RemoveReactionAsync(int userId, int songId);
	}
}
