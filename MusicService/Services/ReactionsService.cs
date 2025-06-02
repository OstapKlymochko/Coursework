using Common.Contracts;
using Common.Errors;
using Common.Result;
using Common.Services.Interfaces;
using FluentValidation;
using MusicService.Models;
using MusicService.Services.Interfaces;

namespace MusicService.Services
{
    public class ReactionsService : IReactionsService
    {
        private readonly IReactionsDbService _reactionsDbService;
        private readonly IValidator<CreateReactionModel> _reactionModelValidator;
        private readonly ICollectionsDbService _collectionsDbService;
        private readonly IMessageQueueService _messageQueueService;

        public ReactionsService(IReactionsDbService reactionsDbService, IValidator<CreateReactionModel> reactionModelValidator, ICollectionsDbService collectionsDbService, IMessageQueueService messageQueueService)
        {
            _reactionsDbService = reactionsDbService;
            _reactionModelValidator = reactionModelValidator;
            _collectionsDbService = collectionsDbService;
            _messageQueueService = messageQueueService;
        }

        public async Task<ServiceResult<int>> CreateReactionAsync(CreateReactionModel reaction, int userId)
        {
            var validationResult = _reactionModelValidator.Validate(reaction);
            if (!validationResult.IsValid) return new ModelError(string.Join(", ", validationResult.Errors));

            var interactionMessage = new SongInteractionContract()
            {
                UserId = userId,
                InteractionType = reaction.Type.ToLower(),
                SongId = reaction.SongId
            };

            var reactionDbRecord = await _reactionsDbService.GetUserSongReactionAsync(userId, reaction.SongId);
            if (reactionDbRecord == null) await _reactionsDbService.AddReactionAsync(reaction, userId);
            else if (reactionDbRecord.Type != reaction.Type)
            {
                await _reactionsDbService.ToggleReactionAsync(reaction.SongId, userId);
                interactionMessage.Toggle = true;
            }

            if (reaction.Type == "like") await _collectionsDbService.AddSongToCollectionAsync(reaction.SongId, userId, 0);
            else await _collectionsDbService.RemoveSongFromCollectionAsync(reaction.SongId, userId, 0);

            await _messageQueueService.PublishMessageAsync(interactionMessage);

            return 0;
        }

        public async Task RemoveReactionAsync(int userId, int songId)
        {
            var reaction = await _reactionsDbService.RemoveReactionAsync(songId, userId);
            
            var interactionMessage = new SongInteractionContract()
            {
                UserId = userId,
                InteractionType = reaction.Type.ToLower(),
                SongId = reaction.SongId,
                Delete = true
            };
            
            if (reaction.Type == "like") await _collectionsDbService.RemoveSongFromCollectionAsync(songId, userId, 0);
            
            await _messageQueueService.PublishMessageAsync(interactionMessage);
        }
    }
}
