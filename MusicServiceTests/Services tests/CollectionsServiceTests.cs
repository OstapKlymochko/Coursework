using Common.CommonTypes;
using Common.Services.Interfaces;
using FluentValidation;
using Moq;
using MusicService.Models;
using MusicService.Services;
using MusicService.Services.Interfaces;

namespace MusicServiceTests.Services_tests
{
    public class CollectionsServiceTests
    {
        private readonly Mock<ICollectionsDbService> _collectionsDbServiceMock;
        private readonly Mock<IFileLinkGeneratorService> _filesServiceMock;
        private readonly Mock<IValidator<CreateCollectionModel>> _collectionModelValidatorMock;

        private readonly ICollectionsService _collectionsService;

        public CollectionsServiceTests()
        {
            _collectionsDbServiceMock = new Mock<ICollectionsDbService>();
            _collectionModelValidatorMock = new Mock<IValidator<CreateCollectionModel>>();
            _filesServiceMock = new Mock<IFileLinkGeneratorService>();

            _collectionsService = new CollectionsService(_collectionsDbServiceMock.Object, _collectionModelValidatorMock.Object, _filesServiceMock.Object);
        }


        [Fact]
        public async Task MusicService_CreateCollection()
        {
            var collection = new CreateCollectionModel() { Type = "Playlist", OwnerUserName = "okl", Title = "Likes" };

            _collectionModelValidatorMock.Setup(c => c.Validate(collection)).Returns(new FluentValidation.Results.ValidationResult());
            _collectionsDbServiceMock.Setup(c => c.GetCollectionByNameAsync(collection.Title, 1)).ReturnsAsync((CollectionDbModel)null!);
            _collectionsDbServiceMock.Setup(c => c.CreateCollectionAsync(GetAny<CollectionDbModel>())).Returns(Task.FromResult((object)null!)).Verifiable();

            var result = await _collectionsService.CreateCollection(collection, 1);

            Assert.True(result.IsSuccess);
            Assert.IsType<BasicResponse>(result.Value);
        }

        [Fact]
        public async Task MusicService_GetUsersCollectionsAsync()
        {
            _collectionsDbServiceMock.Setup(c => c.GetUserCollectionsAsync(1)).ReturnsAsync(new List<CollectionDto>()
            {
                new CollectionDto()
                {
                    Id = 1,
                    ImageUrl= "testtest",
                    OwnerId = 1,
                    OwnerUserName = "okl",
                    SongsCount = 1,
                    Title="test"
                }
            });
            _filesServiceMock.Setup(f => f.GetPreSignedUrl(GetAny<string>())).ReturnsAsync(GetAny<string>());

            var result = await _collectionsService.GetUsersCollectionsAsync(1);

            Assert.True(result.IsSuccess);
            Assert.IsType<CollectionsListDto>(result.Value);
            Assert.NotNull(result.Value.Collections);
            Assert.NotEmpty(result.Value.Collections);
            Assert.Contains(result.Value.Collections, c => c.Id == 1);
        }

        [Fact]
        public async Task UserService_RemoveCollectionAsync()
        {
            _collectionsDbServiceMock.Setup(c => c.GetCollectionByIdAsync(1, 1)).ReturnsAsync(new CollectionDbModel()
            {
                Id = 1,
            });
            _collectionsDbServiceMock.Setup(c => c.DeleteCollectionAsync(1, 1)).Returns(Task.FromResult((object)null!)).Verifiable();

            var result = await _collectionsService.RemoveCollectionAsync(1, 1);

            Assert.True(result.IsSuccess);
            Assert.IsType<BasicResponse>(result.Value);
        }

        [Fact]
        public async Task UserService_GetCollectionSongsAsync()
        {
            _collectionsDbServiceMock.Setup(c => c.GetCollectionByIdAsync(1, 1)).ReturnsAsync(new CollectionDbModel()
            {
                Id = 1,
            });
            _collectionsDbServiceMock.Setup(c => c.GetCollectionSongsAsync(1, 1)).ReturnsAsync(new List<SongDto>()
            {
                new SongDto()
                {
                    Id = 1,
                    LogoUrl = "testtets"
                }
            });
            _filesServiceMock.Setup(f => f.GetPreSignedUrl(GetAny<string>())).ReturnsAsync("https://testtest");

            var result = await _collectionsService.GetCollectionSongsAsync(1, 1);
            Assert.True(result.IsSuccess);
            Assert.IsType<SongsListDto>(result.Value);
            Assert.NotNull(result.Value.Songs);
            Assert.NotEmpty(result.Value.Songs);
            Assert.Contains(result.Value.Songs, c => c.Id == 1);
        }

        [Fact]
        public async Task UserService_AddSongToCollectionAsync()
        {
            _collectionsDbServiceMock.Setup(c => c.GetCollectionByIdAsync(1, 1)).ReturnsAsync(new CollectionDbModel()
            {
                Id = 1,
                Type = "Album"
            });
            _collectionsDbServiceMock.Setup(c => c.IsSongInCollection(1, 1, 1)).ReturnsAsync(false);
            _collectionsDbServiceMock.Setup(c => c.AddSongToCollectionAsync(1, 1, 1)).Returns(Task.FromResult((object)null!)).Verifiable();

            var result = await _collectionsService.AddSongToCollectionAsync(1, 1, 1, true);

            Assert.True(result.IsSuccess);
            Assert.IsType<BasicResponse>(result.Value);
        }
        
        [Fact]
        public async Task UserService_RemoveSongFromCollectionAsync()
        {
            _collectionsDbServiceMock.Setup(c => c.GetCollectionByIdAsync(1, 1)).ReturnsAsync(new CollectionDbModel()
            {
                Id = 1,
                Type = "Album"
            });
            _collectionsDbServiceMock.Setup(c => c.IsSongInCollection(1, 1, 1)).ReturnsAsync(true);
            _collectionsDbServiceMock.Setup(c => c.RemoveSongFromCollectionAsync(1, 1, 1)).Returns(Task.FromResult((object)null!)).Verifiable();

            var result = await _collectionsService.RemoveSongFromCollectionAsync(1, 1, 1, true);

            Assert.True(result.IsSuccess);
            Assert.IsType<BasicResponse>(result.Value);
        }

        private T GetAny<T>() => It.IsAny<T>();

    }
}
