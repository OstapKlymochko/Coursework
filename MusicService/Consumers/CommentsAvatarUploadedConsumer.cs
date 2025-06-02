using Common.Contracts;
using Common.Services.Interfaces;
using MassTransit;

namespace MusicService.Consumers
{
    public class CommentsAvatarUploadedConsumer : IConsumer<AvatarUploadedContract>
    {
        private readonly IDataAccessService _dataAccessService;

        public CommentsAvatarUploadedConsumer(IDataAccessService dataAccessService)
        {
            _dataAccessService = dataAccessService;
        }

        public Task Consume(ConsumeContext<AvatarUploadedContract> context)
        {
            string query = "update comments set avatarfilekey = @FileKey where userid = @UserId";
            return _dataAccessService.ExecuteStatementAsync(query, context.Message);
        }
    }
}
