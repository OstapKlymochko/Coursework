using FluentValidation;
using Moq;
using MusicService.Models;
using MusicService.Services;
using MusicService.Services.Interfaces;

namespace MusicServiceTests.Services_tests
{
    public class ReactionsServiceTests
    {
        private readonly Mock<IReactionsDbService> _reactionsDbServiceMock;
        private readonly Mock<IValidator<CreateReactionModel>> _reactionModelValidatorMock;
        private readonly Mock<ICollectionsDbService> _collectionsDbServiceMock;

        private readonly IReactionsService _reactionsService;

        public ReactionsServiceTests()
        {
            _reactionsDbServiceMock = new Mock<IReactionsDbService>();
            _collectionsDbServiceMock = new Mock<ICollectionsDbService>();
            _reactionModelValidatorMock = new Mock<IValidator<CreateReactionModel>>();

            _reactionsService = new ReactionsService(_reactionsDbServiceMock.Object, _reactionModelValidatorMock.Object, _collectionsDbServiceMock.Object);
        }

        [Fact]
        public async Task ReactionsService_CreateReactionAsync()
        {
            var model = new CreateReactionModel()
            {
                SongId = 1,
                Type = "like"
            };

            _reactionModelValidatorMock.Setup(r => r.Validate(model)).Returns(new FluentValidation.Results.ValidationResult());
            _reactionsDbServiceMock.Setup(r => r.GetUserSongReactionAsync(1, 1)).ReturnsAsync((ReactionDbModel)null!);
            _reactionsDbServiceMock.Setup(r => r.AddReactionAsync(model, 1)).Returns(Task.CompletedTask).Verifiable();
            _collectionsDbServiceMock.Setup(c => c.AddSongToCollectionAsync(1, 1, 0));

            var result = await _reactionsService.CreateReactionAsync(model, 1);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(0, result.Value);
        }

        [Fact]
        public async Task ReactionsService_RemoveReactionAsync()
        {
            _reactionsDbServiceMock.Setup(r => r.RemoveReactionAsync(1, 1)).ReturnsAsync(new ReactionDbModel()
            {
                SongId = 1,
                Type = "like",
                UserId = 1
            });
            _collectionsDbServiceMock.Setup(c => c.RemoveSongFromCollectionAsync(1, 1, 0)).Returns(Task.CompletedTask).Verifiable();

            await _reactionsService.RemoveReactionAsync(1, 1);
        }


        private T GetAny<T>() => It.IsAny<T>();

    }
}
