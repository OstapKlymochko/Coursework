namespace Common.Services;

public interface IDataAccessService
{
	public Task<IEnumerable<TResult>> QueryDataAsync<TResult, TParameters>(string query, TParameters? parameters) where TParameters : class? where TResult : class;

	public Task<TResult?> QuerySingleRecordAsync<TResult, TParameters>(string query, TParameters? parameters) where TParameters : class? where TResult: class;

	public Task ExecuteStatementAsync<TParameters>(string query, TParameters? parameters) where TParameters : class;
}