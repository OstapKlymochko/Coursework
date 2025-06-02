using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Common.Services.Interfaces;

namespace Common.Services
{
    public class FileLinkGeneratorService : IFileLinkGeneratorService
    {
        private readonly AmazonS3Client _s3Client;
        private readonly string _bucketName;

        public FileLinkGeneratorService()
        {
            _s3Client = new AmazonS3Client();
            _bucketName = string.Empty;
        }

        public FileLinkGeneratorService(string client, string secret, string bucket)
        {
            var credentials = new BasicAWSCredentials(client, secret);
            _bucketName = bucket;
            _s3Client = new AmazonS3Client(credentials, RegionEndpoint.EUNorth1);

        }

        public Task<string?> GetPreSignedUrl(string fileKey)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = fileKey,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddDays(1)
            };
            return _s3Client.GetPreSignedURLAsync(request);
        }
    }
}
