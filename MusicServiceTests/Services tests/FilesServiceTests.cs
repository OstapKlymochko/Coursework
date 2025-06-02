using Microsoft.AspNetCore.Http;
using Moq;
using MusicService.Services;
using MusicService.Services.Interfaces;
using System.Text;

namespace MusicServiceTests.Services_tests
{
    public class FilesServiceTests
    {
        private readonly Mock<IS3Service> _s3ServiceMock;
        private readonly IFilesService _filesService;

        public FilesServiceTests()
        {
            _s3ServiceMock = new Mock<IS3Service>();
            _filesService = new FilesService(_s3ServiceMock.Object);
        }

        //[Fact]
        public async Task FilesService_SaveFileAsync()
        {
            _s3ServiceMock.Setup(f => f.UploadFileAsync(GetAny<MemoryStream>(), GetAny<string>())).Returns(Task.FromResult((object)null!)).Verifiable();

            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");
            IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.txt")
            {
                ContentType = "image/jpeg"
            };
            var a = file.ContentType;
            var result = await _filesService.SaveFileAsync(file);

        }

        private T GetAny<T>() => It.IsAny<T>();
    }
}
