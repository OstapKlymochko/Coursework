using Common.CommonTypes;
using Common.Errors;
using Common.Result;
using Common.Services.Interfaces;
using MassTransit;

namespace Common.Services
{
    public class MessageQueueService : IMessageQueueService
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IScopedClientFactory _scopedClientFactory;

        public MessageQueueService(IPublishEndpoint publishEndpoint, IScopedClientFactory scopedClientFactory)
        {
            _publishEndpoint = publishEndpoint;
            _scopedClientFactory = scopedClientFactory;
        }

        public Task PublishMessageAsync<TMessage>(TMessage message) where TMessage : class
        {
            return _publishEndpoint.Publish(message);
        }

        public async Task<TError?> PublishMessageAndHandleErrorAsync<TMessage, TError>(TMessage message) where TMessage : class where TError : class
        {
            var client = _scopedClientFactory.CreateRequestClient<TMessage>();
            var response = await client.GetResponse<TError, BasicResponse>(message);
            if(response.Is<TError>(out var r)) return r.Message;
            return null;
        }

        public async Task<TResponse?> QueryDataAsync<TMessage, TResponse>(TMessage message) where TMessage : class where TResponse : class
        {
            var client = _scopedClientFactory.CreateRequestClient<TMessage>();
            var response = await client.GetResponse<TResponse>(message);
            return response.Message;
        }

        public async Task<ServiceResult<TResponse?>> QueryDataAsync<TMessage, TResponse, TError>(TMessage message)
            where TMessage : class
            where TResponse : class
            where TError : BaseError
        {
            var client = _scopedClientFactory.CreateRequestClient<TMessage>();
            var response = await client.GetResponse<TResponse, TError>(message);
            if (response.Is(out Response<TResponse>? result)) return result.Message;
            if (response.Is(out Response<TError>? error)) return error.Message;
            return new InternalError("Something went wrong");
        }
    }
}
