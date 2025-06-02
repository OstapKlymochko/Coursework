using Common.Services.Interfaces;
using FluentValidation;
using Moq;
using MusicService.Models;
using MusicService.Services;
using MusicService.Services.Interfaces;

namespace MusicServiceTests.Services_tests
{
    public class CommentsServiceTests
    {
        private readonly Mock<ICommentsDbService> _commentsDbService;
        private readonly Mock<IFileLinkGeneratorService> _fileLinkGeneratorService;
        private readonly Mock<IValidator<CreateCommentModel>> _createCommentValidator;
        private readonly Mock<IValidator<UpdateCommentModel>> _updateCommentValidator;

        private ICommentsService _commentsService;

        public CommentsServiceTests()
        {
            _commentsDbService = new Mock<ICommentsDbService>();
            _fileLinkGeneratorService = new Mock<IFileLinkGeneratorService>();
            _createCommentValidator = new Mock<IValidator<CreateCommentModel>>();
            _updateCommentValidator = new Mock<IValidator<UpdateCommentModel>>();

            _commentsService = new CommentsService(_commentsDbService.Object, _updateCommentValidator.Object, _createCommentValidator.Object, _fileLinkGeneratorService.Object);
        }

        [Fact]
        public async Task CommentsService_GetCommentsCountAsync()
        {
            int songId = 1;
            _commentsDbService.Setup(c => c.GetCommentsCountAsync(songId)).ReturnsAsync("1");

            var result = await _commentsService.GetCommentsCountAsync(songId);

            Assert.IsType<int>(result);
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task CommentsService_GetSongCommentsPaginated()
        {
            int songId = 1;
            int select = 10;
            int skip = 0;

            _commentsDbService.Setup(c => c.GetSongCommentsPaginatedAsync(songId, select, skip)).ReturnsAsync(new List<CommentDto>()
            {
                new CommentDto()
                {
                    Id = 1,
                    AvatarFileKey = "test"
                }
            });
            _commentsDbService.Setup(c => c.GetCommentsCountAsync(songId, null)).ReturnsAsync("1");
            _fileLinkGeneratorService.Setup(f => f.GetPreSignedUrl(GetAny<string>())).ReturnsAsync("testtest");

            var result = await _commentsService.GetSongCommentsPaginated(songId, select, skip);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Comments);
            Assert.Equal(1, result.PagesCount);
            Assert.Contains(result.Comments, c => c.Id == 1);
        }

        [Fact]
        public async Task CommentsService_CreateCommentAsync()
        {
            CreateCommentModel comment = new()
            {
                Body = "test",
                SongId = 2,
                Username = "okl",
                RepliedTo = 1
            };

            _createCommentValidator.Setup(c => c.Validate(comment)).Returns(new FluentValidation.Results.ValidationResult());
            _commentsDbService.Setup(c => c.GetCommentByIdAsync(1)).ReturnsAsync(new CommentDbModel()
            {
                Id = 1,
                RepliedTo = 0
            });

            _commentsDbService.Setup(c => c.CreateCommentAsync(GetAny<CommentDbModel>())).ReturnsAsync(new CommentDto() { Id = 2, RepliesCount = 0 });

            var result = await _commentsService.CreateCommentAsync(comment, 1);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Id);
        }

        public async Task CommentsService_UpdateCommentAsync()
        {
            UpdateCommentModel comment = new()
            {
                CommentId = 1,
                Body = "test",
            };

            _updateCommentValidator.Setup(c => c.Validate(comment)).Returns(new FluentValidation.Results.ValidationResult());
            _commentsDbService.Setup(c => c.UpdateCommentBodyAsync(comment)).Returns(Task.FromResult((object)null!)).Verifiable();

            var result = await _commentsService.UpdateCommentAsync(comment, 1);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(0, result.Value);
        }

        private T GetAny<T>() => It.IsAny<T>();
    }
}
