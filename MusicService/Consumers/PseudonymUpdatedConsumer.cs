using Common.Contracts;
using Common.Services.Interfaces;
using MassTransit;

namespace MusicService.Consumers
{
	public class PseudonymUpdatedConsumer : IConsumer<UsernameUpdatedContract>
	{
		private readonly IDataAccessService _dataAccess;

		public PseudonymUpdatedConsumer(IDataAccessService dataAccess)
		{
			_dataAccess = dataAccess;
		}

		public async Task Consume(ConsumeContext<UsernameUpdatedContract> context)
		{
			string query = "update songs set author_pseudonym = @UserName where authorid = @Id; "
				+ "update collections set ownerusername = @UserName where ownerid = @Id;";
			await _dataAccess.ExecuteStatementAsync(query, context.Message);
		}
	}
}
