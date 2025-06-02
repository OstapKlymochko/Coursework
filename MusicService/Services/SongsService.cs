using Common.CommonTypes;
using Common.CommonTypes.Interfaces;
using Common.Errors;
using Common.Result;
using MusicService.Models;
using MusicService.Services.Interfaces;
using FluentValidation;
using Common.Services.Interfaces;
using Common.Contracts;

namespace MusicService.Services
{
    public class SongsService : ISongsService
    {
        private readonly ISongsDbService _songsDbService;
        private readonly IFileLinkGeneratorService _filesService;
        private readonly IGenericRepository<GenreDbModel> _genresRepository;
        private readonly ICommentsService _commentsService;
        private readonly IMessageQueueService _messageQueueService;
        private readonly string _collectionsServiceAddress;

        public SongsService(ISongsDbService songsDbService, IFileLinkGeneratorService filesService, 
            IGenericRepository<GenreDbModel> genresRepository, ICommentsService commentsService, IConfiguration _configuration, IMessageQueueService messageQueueService)
        {
            _songsDbService = songsDbService;
            _filesService = filesService;
            _genresRepository = genresRepository;
            _commentsService = commentsService;
            _messageQueueService = messageQueueService;
            _collectionsServiceAddress = _configuration.GetValue<string>("RecsServiceAddress:Address")!;
        }

        public async Task<ServiceResult<SongDetailsDto>> GetSongById(int id, int userId)
        {
            var song = await _songsDbService.GetSongByIdAsync(id, userId);
            if (song == null) return new NotFoundError("Song does not exist");
            var commentsCount = await _commentsService.GetCommentsCountAsync(id);
            song.Comments = commentsCount;
            //var song = new SongDetailsDto()
            //{
            //    Id = id,
            //    Comments = commentsCount,
            //    Dislikes = songDbRecord.Dislikes,
            //    Likes = songDbRecord.Likes,
            //    IsDisliked = songDbRecord.IsDisliked,
            //    IsLiked = songDbRecord.IsLiked
            //};
            song.LogoUrl = (await _filesService.GetPreSignedUrl(song.LogoUrl))!;
            song.SongUrl = (await _filesService.GetPreSignedUrl(song.SongUrl))!;
            if (song.VideoClipUrl != null) song.VideoClipUrl = await _filesService.GetPreSignedUrl(song.VideoClipUrl);
            return song;
        }

        public async Task<ServiceResult<BasicResponse>> UploadSongAsync(UploadSongModel uploadSongModel, int userId)
        {
            //var validationResult = await _uploadSongValidator.ValidateAsync(uploadSongModel);
            //if (!validationResult.IsValid) return new ModelError(string.Join(", ", validationResult.Errors));

            //var genre = await _genresRepository.GetByIdAsync(uploadSongModel.GenreId);
            //if (genre == null) return new ModelError("Genre does not exist");

            //var songDbModel = new SongDbModel()
            //{
            //    AuthorPseudonym = uploadSongModel.Pseudonym,
            //    AuthorId = userId,
            //    CreatedAt = DateTime.UtcNow,
            //    Title = uploadSongModel.Title,
            //    GenreId = uploadSongModel.GenreId
            //};

            //if (uploadSongModel.Logo != null) songDbModel.LogoFileKey = await _filesService.SaveFileAsync(uploadSongModel.Logo);
            //if (uploadSongModel.VideoClip != null) songDbModel.VideoClipFileKey = await _filesService.SaveFileAsync(uploadSongModel.VideoClip);
            //songDbModel.SongFileKey = await _filesService.SaveFileAsync(uploadSongModel.Song);
            //await _songsDbService.SaveSongAsync(songDbModel);

            return new BasicResponse("Song was successfully uploaded");
        }

        public async Task<ServiceResult<SongsListDto>> GetSongsPaginatedAsync(uint select = 20, uint skip = 0, string key = "")
        {
            if (key != string.Empty) key = string.Join(" & ", key.Trim().Split(' '));
            var songsList = await _songsDbService.GetSongListPaginatedAsync((int)select, (int)skip, key);
            foreach (var s in songsList)
            {
                var logoUrl = await _filesService.GetPreSignedUrl(s.LogoUrl);
                s.LogoUrl = logoUrl ?? null!;
            }
            var songsCountModel = await _songsDbService.GetSongsCount(key);
            if (key != string.Empty) songsList = songsList.Where(s => Math.Round((decimal)s.Rank, 2) != 0).ToList();
            var result = new SongsListDto
            {
                Songs = songsList,
                PagesCount = (int)Math.Ceiling((double)songsCountModel.Count / (double)select)
            };

            return result;
        }

        public Task<List<GenreDbModel>> GetGenres() => _genresRepository.GetAllAsync();

        public async Task<ServiceResult<SongKeysDto>> GetSongNamesByKey(string key)
        {
            IEnumerable<string> titles;
            if (key == string.Empty) titles = new string[] { };
            else
            {
                key = string.Join(" & ", key.Trim().Split(' '));
                titles = await _songsDbService.GetSongNamesByKeyAsync(key);
            }
            return new SongKeysDto() { Titles = titles };
        }

        public async Task<ServiceResult<SongStatisticsData>> GetSongStatisticsAsync(int songId, int userId)
        {
            var statistics = await _songsDbService.GetSongStatisticsAsync(songId, userId);
            if (statistics == null) return new NotFoundError("Song was not found");
            int commentsCount = await _commentsService.GetCommentsCountAsync(songId);
            statistics.Comments = commentsCount;
            return statistics;
        }

        public async Task<ServiceResult<int>> RegisterSongListened(SongPlayDto songInteractionDto)
        {
            var song = await _songsDbService.GetSongByIdAsync(songInteractionDto.SongId, songInteractionDto.UserId);
            if (song == null) return new NotFoundError("Song does not exist");

            var model = new SongInteractionContract()         
            {
                UserId = songInteractionDto.UserId,
                SongId = songInteractionDto.SongId,
                InteractionType = "play",
                ListenTime = songInteractionDto.ListenTime,
            };
            await _messageQueueService.PublishMessageAsync(model);
            
            return 0;
        }
    }
}
