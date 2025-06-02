using Amazon.Runtime;
using Amazon.S3.Model;
using Amazon.S3;
using Amazon;
using FilesService.Services.Interface;
using System.IO;

namespace FilesService.Services
{

    public class S3Service : IS3Service
    {
        private readonly AmazonS3Client _s3Client;
        private readonly string _bucketName;

        public S3Service(IConfiguration configuration)
        {
            var credentials = new BasicAWSCredentials(
                configuration.GetValue<string>("S3:ClientId"),
                configuration.GetValue<string>("S3:SecretKey"));
            _bucketName = configuration.GetValue<string>("S3:Bucket")!;
            _s3Client = new AmazonS3Client(credentials, RegionEndpoint.EUNorth1);
        }

        public async Task<GetObjectResponse?> GetFileAsync(string fileName)
        {
            try
            {
                return await _s3Client.GetObjectAsync(_bucketName, fileName);
            }
            catch
            {
                return null;
            }

        }

        public async Task UploadFileAsync(IFormFile file, string fileName)
        {
            var request = new PutObjectRequest()
            {
                BucketName = _bucketName,
                Key = fileName,
                InputStream = file.OpenReadStream()
            };
            await _s3Client.PutObjectAsync(request);
        }

        public async Task UploadFileAsync(MemoryStream stream, string fileName)
        {
            var request = new PutObjectRequest()
            {
                BucketName = _bucketName,
                Key = fileName,
                InputStream = stream
            };
            await _s3Client.PutObjectAsync(request);
        }

        public async Task<string?> GetPreSignedUrl(string fileKey)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = fileKey,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddDays(1)
            };
            return await _s3Client.GetPreSignedURLAsync(request);
        }

        public Task DeleteFileAsync(string fileKey)
        {
            var request = new DeleteObjectRequest()
            {
                BucketName = _bucketName,
                Key = fileKey,
            };
            return _s3Client.DeleteObjectAsync(request);
        }
    }
}