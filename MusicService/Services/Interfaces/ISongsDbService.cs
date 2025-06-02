using MusicService.Models;

namespace MusicService.Services.Interfaces
{
	public interface ISongsDbService
	{
		public Task<SongDetailsDto?> GetSongByIdAsync(int id, int userId);
        public Task<IEnumerable<SongDto>> GetSongListPaginatedAsync(int select = 10, int skip = 0, string key = "");
		public Task SaveSongAsync(SongDbModel song);
		public Task<SongsCountModel> GetSongsCount(string key = "");
		public Task<IEnumerable<string>> GetSongNamesByKeyAsync(string key, int select = 10);
		public Task<SongStatisticsData?> GetSongStatisticsAsync(int songId, int userId);
		public Task RegisterSongListened(SongDbListeningModel listeningModel);
    }
}
