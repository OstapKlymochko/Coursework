using Common.Services.Interfaces;
using Dapper;

namespace Common.Services
{
    public class DataAccessService : IDataAccessService
	{
		private readonly IDbContext _ctx;
		public DataAccessService(IDbContext ctx)
		{
			_ctx = ctx;
		}

		public async Task<IEnumerable<TResult>> QueryDataAsync<TResult, TParameters>(string query, TParameters? parameters = null) where TResult : class where TParameters : class?
		{
			using var connection = _ctx.CreateConnection();
			return await connection.QueryAsync<TResult>(query, parameters);
		}

		public async Task<TResult?> QuerySingleRecordAsync<TResult, TParameters>(string query, TParameters? parameters = null) where TResult : class where TParameters : class?
		{
			using var connection = _ctx.CreateConnection();
			return await connection.QueryFirstOrDefaultAsync<TResult>(query, parameters);
		}

		public async Task ExecuteStatementAsync<TParameters>(string query, TParameters? parameters = null) where TParameters : class
		{
			using var connection = _ctx.CreateConnection();
			await connection.ExecuteAsync(query, parameters);
		}

		public async Task<TReturn> ExecuteStatementAndReturnAsync<TReturn, TParameters>(string query, TParameters? parameters = null) where TParameters : class
		{
			using var connection = _ctx.CreateConnection();
			var res = await connection.QueryAsync<TReturn>(query, parameters);
			return res.Single();
		}

	}
}
