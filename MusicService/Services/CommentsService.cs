using Common.Errors;
using Common.Result;
using Common.Services.Interfaces;
using FluentValidation;
using MusicService.Models;
using MusicService.Services.Interfaces;

namespace MusicService.Services
{
    public class CommentsService : ICommentsService
    {
        private readonly ICommentsDbService _commentsDbService;
        private readonly IFileLinkGeneratorService _fileLinkGeneratorService;
        private readonly IValidator<CreateCommentModel> _createCommentValidator;
        private readonly IValidator<UpdateCommentModel> _updateCommentValidator;
        public CommentsService(ICommentsDbService commentsDbService, IValidator<UpdateCommentModel> updateCommentValidator,
            IValidator<CreateCommentModel> createCommentValidator, IFileLinkGeneratorService fileLinkGeneratorService)
        {
            _commentsDbService = commentsDbService;
            _updateCommentValidator = updateCommentValidator;
            _createCommentValidator = createCommentValidator;
            _fileLinkGeneratorService = fileLinkGeneratorService;
        }
        //TODO remove
        public async Task<int> GetCommentsCountAsync(int songId)
        {
            string? count = await _commentsDbService.GetCommentsCountAsync(songId);
            if (count == null) return 0;
            return int.Parse(count);
        }

        public async Task<CommentsListModel> GetSongCommentsPaginated(int songId, int select = 10, int skip = 0, int? parrentCommentId = null)
        {
            IEnumerable<CommentDto> comments;
            if (parrentCommentId == null) comments = await _commentsDbService.GetSongCommentsPaginatedAsync(songId, select, skip);
            else comments = await _commentsDbService.GetCommentRepliesPaginatedAsync((int)parrentCommentId, select, skip);
            string? countString = await _commentsDbService.GetCommentsCountAsync(songId, parrentCommentId);
            int count = int.Parse(countString!);
            foreach(var c in comments)
            {
                if (c.AvatarFileKey != null) c.AvatarFileKey = await _fileLinkGeneratorService.GetPreSignedUrl(c.AvatarFileKey);
            }
            return new CommentsListModel()
            {
                Comments = comments,
                PagesCount = (int)Math.Ceiling((double)count / (double)select)
            };
        }

        public async Task<ServiceResult<CommentDto>> CreateCommentAsync(CreateCommentModel comment, int userId)
        {
            var validationResult = _createCommentValidator.Validate(comment);
            if (!validationResult.IsValid) return new ModelError(string.Join(", ", validationResult.Errors));

            var commentDbModel = new CommentDbModel(comment);
            if (comment.RepliedTo != null)
            {
                var repliedToComment = await _commentsDbService.GetCommentByIdAsync((int)comment.RepliedTo);
                if (repliedToComment == null) return new NotFoundError("Could not find comment");
                else if (repliedToComment.RepliedTo != null) commentDbModel.RepliedTo = repliedToComment.RepliedTo;
                else commentDbModel.RepliedTo = comment.RepliedTo;
            }
            commentDbModel.UserName = comment.Username;
            commentDbModel.UserId = userId;

            var c = await _commentsDbService.CreateCommentAsync(commentDbModel);
            return c;
        }

        public async Task<ServiceResult<int>> UpdateCommentAsync(UpdateCommentModel comment, int userId)
        {
            var validationResult = _updateCommentValidator.Validate(comment);
            if (!validationResult.IsValid) return new ModelError(string.Join(", ", validationResult.Errors));

            await _commentsDbService.UpdateCommentBodyAsync(comment);
            return 0;
        }

        public Task DeleteCommentAsync(int commentId) =>
            _commentsDbService.DeleteCommentAsync(commentId);

    }
}
