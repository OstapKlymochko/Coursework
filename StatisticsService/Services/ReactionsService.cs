using Common.Contracts;
using Common.Errors;
using Common.Result;
using Common.Services.Interfaces;
using FluentValidation;
using StatisticsService.Models;
using StatisticsService.Services.Interfaces;

namespace StatisticsService.Services
{
    public class ReactionsService : IReactionsService
    {
        private readonly IReactionsDbService _reactionsDbService;
        private readonly IMessageQueueService _messageQueueService;
        private readonly IValidator<CreateReactionModel> _reactionModelValidator;

        public ReactionsService(IReactionsDbService reactionsDbService, IValidator<CreateReactionModel> reactionModelValidator, IMessageQueueService messageQueueService)
        {
            _reactionsDbService = reactionsDbService;
            _reactionModelValidator = reactionModelValidator;
            _messageQueueService = messageQueueService;
        }

        public async Task<ServiceResult<int>> CreateReactionAsync(CreateReactionModel reaction, int userId)
        {
            var validationResult = _reactionModelValidator.Validate(reaction);
            if (!validationResult.IsValid) return new ModelError(string.Join(", ", validationResult.Errors));

            var reactionDbRecord = await _reactionsDbService.GetUserSongReactionAsync(userId, reaction.SongId);
            if (reactionDbRecord == null) await _reactionsDbService.AddReactionAsync(reaction, userId);
            else if (reactionDbRecord.Type != reaction.Type) await _reactionsDbService.ToggleReactionAsync(reaction.SongId, userId);

            await PublishReactionsCountChangedMessageAsync(reaction.SongId);
            return 0;
        }

        public async Task RemoveReactionAsync(int userId, int songId)
        {
            await _reactionsDbService.RemoveReactionAsync(songId, userId);
            await PublishReactionsCountChangedMessageAsync(songId);
        }


        private async Task PublishReactionsCountChangedMessageAsync(int songId)
        {
            var reactionsCount = await _reactionsDbService.GetSongReactionsDataAsync(songId);
            var message = new SongStatisticsUpdated() { SongId = songId, Likes = reactionsCount?.Likes ?? 0, Dislikes = reactionsCount?.Dislikes ?? 0 };
            await _messageQueueService.PublishMessageAsync(message);
        }
    }
}
