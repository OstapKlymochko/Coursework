using Common.Contracts;
using Common.Services.Interfaces;
using MassTransit;

namespace MusicService.Consumers
{
	public class MusicServiceUserRegisteredConsumer : IConsumer<UserRegistered>
	{
		private readonly IDataAccessService _dataAccessService;
		public MusicServiceUserRegisteredConsumer(IDataAccessService dataAccessService)
		{
			_dataAccessService = dataAccessService;
		}

		public async Task Consume(ConsumeContext<UserRegistered> context)
		{
			string query = "insert into collections (id, ownerid, title, ownerusername, type, imgfilekey) VALUES (0, @UserId, 'Вподобання', @UserName, 'Playlist', 'likes_default.png');";
			await _dataAccessService.ExecuteStatementAsync(query, context.Message);
		}
	}
}
