using Common.CommonTypes.Interfaces;
using Common.Contracts;
using MassTransit;
using MusicService.Models;
using MusicService.Services.Interfaces;

namespace MusicService.Consumers
{
    public class SongUploadedConsumer : IConsumer<SongUploadedContract>
    {
        private readonly ISongsDbService _songsDbService;
        private readonly IGenericRepository<GenreDbModel> _genresRepository;

        public SongUploadedConsumer(ISongsDbService songsDbService, IGenericRepository<GenreDbModel> genresRepository)
        {
            _songsDbService = songsDbService;
            _genresRepository = genresRepository;
        }

        public async Task Consume(ConsumeContext<SongUploadedContract> context)
        {
            var message = context.Message;

            var genre = await _genresRepository.GetByIdAsync(message.GenreId);
            if (genre == null) return;

            var song = new SongDbModel()
            {
                AuthorId = message.AuthorId,
                AuthorPseudonym = message.AuthorPseudonym,
                CreatedAt = message.CreatedAt,
                GenreId = message.GenreId,
                LogoFileKey = message.LogoFileKey,
                SongFileKey = message.SongFileKey,
                VideoClipFileKey = message.VideoClipFileKey,
                Title = message.Title,
            };

            await _songsDbService.SaveSongAsync(song);
        }
    }
}
