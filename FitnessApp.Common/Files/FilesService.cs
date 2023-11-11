using System.IO;
using System.Threading.Tasks;
using Minio;

namespace FitnessApp.Common.Files
{
    public class FilesService : IFilesService
    {
        private readonly IMinioClient _minioClient;
        public FilesService(IMinioClient minioClient)
        {
            _minioClient = minioClient;
        }

        public async Task UploadFile(string bucketName, string objectName, Stream stream)
        {
            await EnsureBucket(objectName);
            var args = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length);
            await _minioClient.PutObjectAsync(args).ConfigureAwait(false);
        }

        public async Task<byte[]> DownloadFile(string bucketName, string objectName)
        {
            await EnsureBucket(objectName);
            var memoryStream = new MemoryStream();
            var completed = false;
            var args = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithCallbackStream(x =>
                {
                    x.CopyTo(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    completed = true;
                });
            await _minioClient.GetObjectAsync(args);
            var timeoutCounter = 5;
            while (!completed)
            {
                await Task.Delay(100);
                if (timeoutCounter > 0)
                    timeoutCounter--;
                else
                    break;
            }

            return memoryStream.ToArray();
        }

        public async Task DeleteFile(string bucketName, string objectName)
        {
            await EnsureBucket(objectName);
            var args = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);
            await _minioClient.RemoveObjectAsync(args);
        }

        public static string CreateFileName(string propertyName, string userId)
        {
            return $"{propertyName}_{userId}";
        }

        private async Task EnsureBucket(string bucketName)
        {
            if (!await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName)))
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName)).ConfigureAwait(false);
        }
    }
}
