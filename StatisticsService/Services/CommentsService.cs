using Common.Contracts;
using Common.Errors;
using Common.Result;
using Common.Services.Interfaces;
using FluentValidation;
using StatisticsService.Models;
using StatisticsService.Services.Interfaces;

namespace StatisticsService.Services
{
    public class CommentsService : ICommentsService
    {
        private readonly ICommentsDbService _commentsDbService;
        private readonly IMessageQueueService _messageQueueService;
        private readonly IValidator<CreateCommentModel> _createCommentValidator;
        private readonly IValidator<UpdateCommentModel> _updateCommentValidator;
        public CommentsService(ICommentsDbService commentsDbService, IValidator<UpdateCommentModel> updateCommentValidator,
            IValidator<CreateCommentModel> createCommentValidator, IMessageQueueService messageQueueService)
        {
            _commentsDbService = commentsDbService;
            _updateCommentValidator = updateCommentValidator;
            _createCommentValidator = createCommentValidator;
            _messageQueueService = messageQueueService;
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
            int count;
            if (parrentCommentId == null) comments = await _commentsDbService.GetSongCommentsPaginatedAsync(songId, select, skip);
            else comments = await _commentsDbService.GetCommentRepliesPaginatedAsync((int)parrentCommentId, select, skip);
            string? countString = await _commentsDbService.GetCommentsCountAsync(songId, parrentCommentId);
            count = int.Parse(countString!);
            
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
            commentDbModel.UserId = userId;

            var c = await _commentsDbService.CreateCommentAsync(commentDbModel);
            await PublishCommentsCountChangedMessageAsync(comment.SongId);

            return c;
        }

        public async Task<ServiceResult<int>> UpdateCommentAsync(UpdateCommentModel comment, int userId)
        {
            var validationResult = _updateCommentValidator.Validate(comment);
            if (!validationResult.IsValid) return new ModelError(string.Join(", ", validationResult.Errors));

            await _commentsDbService.UpdateCommentBodyAsync(comment);
            return 0;
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            string songId = await _commentsDbService.DeleteCommentAsync(commentId);
            await PublishCommentsCountChangedMessageAsync(int.Parse(songId));
        }

        private async Task PublishCommentsCountChangedMessageAsync(int songId)
        {
            string? commentsCount = await _commentsDbService.GetCommentsCountAsync(songId);
            if (commentsCount == null) return;
            var message = new SongStatisticsUpdated() { Comments = int.Parse(commentsCount), SongId = songId };
            await _messageQueueService.PublishMessageAsync(message);
        }
    }
}
