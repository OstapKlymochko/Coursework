using Common.CommonTypes;
using Common.Result;
using MusicService.Models;

namespace MusicService.Services.Interfaces
{
    public interface ISongsService
    {
        public Task<ServiceResult<SongDetailsDto>> GetSongById(int id, int userId);
        public Task<ServiceResult<BasicResponse>> UploadSongAsync(UploadSongModel uploadSongModel, int userId);
        public Task<ServiceResult<SongsListDto>> GetSongsPaginatedAsync(uint limit = 20, uint skip = 0, string key = "");
        public Task<List<GenreDbModel>> GetGenres();
        public Task<ServiceResult<SongKeysDto>> GetSongNamesByKey(string key);
        public Task<ServiceResult<SongStatisticsData>> GetSongStatisticsAsync(int songId, int userId);
        public Task<ServiceResult<int>> RegisterSongListened(SongPlayDto songInteractionDto);
    }
}