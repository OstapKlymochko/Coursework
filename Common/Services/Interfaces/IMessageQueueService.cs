using Common.Errors;
using Common.Result;

namespace Common.Services.Interfaces
{
    public interface IMessageQueueService
    {
        public Task PublishMessageAsync<TMessage>(TMessage message) where TMessage : class;

        public Task<TError?> PublishMessageAndHandleErrorAsync<TMessage, TError>(TMessage message)
            where TMessage : class
            where TError : class;

        public Task<TResponse?> QueryDataAsync<TMessage, TResponse>(TMessage message)
            where TMessage : class
            where TResponse : class;

        public Task<ServiceResult<TResponse?>> QueryDataAsync<TMessage, TResponse, TError>(TMessage message)
            where TMessage : class
            where TResponse : class
            where TError : BaseError;
    }
}
