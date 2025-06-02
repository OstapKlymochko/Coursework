using Common.CommonTypes.Interfaces;
using Common.Services.Interfaces;
using FluentValidation;
using Moq;
using MusicService.Models;
using MusicService.Services;
using MusicService.Services.Interfaces;

namespace MusicServiceTests.Services_tests
{
    public class SongsServiceTest
    {
        private readonly Mock<ISongsDbService> _songsDbServiceMock;
        private readonly Mock<IFileLinkGeneratorService> _filesServiceMock;
        private readonly Mock<IGenericRepository<GenreDbModel>> _genresRepositoryMock;
        private readonly Mock<ICommentsService> _commentsServiceMock;

        private readonly ISongsService _songsService;

        public SongsServiceTest()
        {
            _songsDbServiceMock = new Mock<ISongsDbService>();
            _filesServiceMock = new Mock<IFileLinkGeneratorService>();
            _genresRepositoryMock = new Mock<IGenericRepository<GenreDbModel>>();
            _commentsServiceMock = new Mock<ICommentsService>();

            _songsService = new SongsService(_songsDbServiceMock.Object, _filesServiceMock.Object, _genresRepositoryMock.Object, _commentsServiceMock.Object);
        }


        [Fact]
        public async Task SongsService_GetSongById()
        {
            _songsDbServiceMock.Setup(s => s.GetSongByIdAsync(1, 1)).ReturnsAsync(new GetSongByIdModel()
            {
                SongFileKey = "asdqwe",
                AuthorId = 1,
                Dislikes = 1,
                IsDisliked = true,
                LogoFileKey = "asdqwe",
                Id = 1
            });

            _commentsServiceMock.Setup(c => c.GetCommentsCountAsync(1)).ReturnsAsync(1);
            _filesServiceMock.Setup(f => f.GetPreSignedUrl("asdqwe")).ReturnsAsync("https://asdqwe.com");

            var result = await _songsService.GetSongById(1, 1);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public async Task SongsServiec_GetSongsPaginatedAsync()
        {
            _songsDbServiceMock.Setup(s => s.GetSongListPaginatedAsync(10, 0, string.Empty)).ReturnsAsync(new List<SongDto>()
            {
                new SongDto()
                {
                    AuthorId = 1,
                    Id = 1,
                    LogoUrl = "asdqwe"
                }
            });
            _filesServiceMock.Setup(f => f.GetPreSignedUrl("asdqwe")).ReturnsAsync("https://asdqwe.com");
            _songsDbServiceMock.Setup(s => s.GetSongsCount("")).ReturnsAsync(new SongsCountModel() { Count = 1 });

            var result = await _songsService.GetSongsPaginatedAsync(10, 0, string.Empty);


            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.NotEmpty(result.Value.Songs);
            Assert.Contains(result.Value.Songs, s => s.Id == 1);
        }

        [Fact]
        public async Task SongsService_GetSongStatisticsAsync()
        {
            _songsDbServiceMock.Setup(s => s.GetSongStatisticsAsync(1, 1)).ReturnsAsync(new SongStatisticsData() { Dislikes = 1, IsLiked = true, Likes = 1 });
            _commentsServiceMock.Setup(c => c.GetCommentsCountAsync(1)).ReturnsAsync(1);

            var result = await _songsService.GetSongStatisticsAsync(1, 1);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.True(result.Value.IsLiked);
            Assert.False(result.Value.IsDisliked);
            Assert.Equal(1, result.Value.Likes);
            Assert.Equal(1, result.Value.Comments);
        }

        [Fact]
        public async Task SongsService_GetSongNamesByKey()
        {
            string expectedSongTitle = "Rock'N'Roll Train";
            _songsDbServiceMock.Setup(s => s.GetSongNamesByKeyAsync("Rock", 10)).ReturnsAsync(new string[]
            {
                expectedSongTitle
            });

            var result = await _songsService.GetSongNamesByKey("Rock");

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.NotEmpty(result.Value.Titles);
            Assert.Contains(result.Value.Titles, s => s == expectedSongTitle);
        }
    }
}
