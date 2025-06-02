using MusicService.Models;

namespace MusicService.Services.Interfaces
{
    public interface IReactionsDbService
    {
        public Task<SongReactionsModel?> GetSongReactionsDataAsync(int songId);
        public Task AddReactionAsync(CreateReactionModel reaction, int userId);
        public Task<ReactionDbModel> RemoveReactionAsync(int songId, int userId);
        public Task ToggleReactionAsync(int songId, int userId);
        public Task<bool> ReactionAlreadyExists(int songId, int userId);
        public Task<ReactionDbModel?> GetUserSongReactionAsync(int userId, int songId);
    }
}
