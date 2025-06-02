using Common.Contracts;
using FilesService.Services.Interface;
using MassTransit;

namespace FilesService.Consumers
{
    public class DeleteFileConsumer : IConsumer<DeleteFileContract>
    {
        private readonly IFilesService _filesService;

        public DeleteFileConsumer(IFilesService filesService)
        {
            this._filesService = filesService;
        }

        public async Task Consume(ConsumeContext<DeleteFileContract> context)
        {
            var message = context.Message;
            await _filesService.DeleteFileAsync(message.FileKey);
        }
    }
}
