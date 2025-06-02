namespace Common.Services.Interfaces;

public interface IDataAccessService
{
    public Task<IEnumerable<TResult>> QueryDataAsync<TResult, TParameters>(string query, TParameters? parameters) where TParameters : class? where TResult : class;

    public Task<TResult?> QuerySingleRecordAsync<TResult, TParameters>(string query, TParameters? parameters) where TParameters : class? where TResult : class;

    public Task ExecuteStatementAsync<TParameters>(string query, TParameters? parameters) where TParameters : class;
    public Task<TReturn> ExecuteStatementAndReturnAsync<TReturn, TParameters>(string query, TParameters? parameters) where TParameters : class;
}