using System.IO;
using System.Threading.Tasks;
using Minio;
using Minio.DataModel.Args;

namespace FitnessApp.Common.Files;

public class FilesService(IMinioClient minioClient) : IFilesService
{
    public async Task UploadFile(string bucketName, string objectName, Stream stream)
    {
        await EnsureBucket(bucketName);
        var args = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length);
        await minioClient.PutObjectAsync(args).ConfigureAwait(false);
    }

    public async Task<byte[]> DownloadFile(string bucketName, string objectName)
    {
        await EnsureBucket(bucketName);
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
        await minioClient.GetObjectAsync(args);
        var timeoutCounter = 5;
        while (!completed)
        {
            await Task.Delay(100);
#pragma warning disable S2589 // Bug with sonar
            if (timeoutCounter > 0)
                timeoutCounter--;
            else
                break;
#pragma warning restore S2589 // Bug with sonar
        }

        return memoryStream.ToArray();
    }

    public async Task DeleteFile(string bucketName, string objectName)
    {
        await EnsureBucket(bucketName);
        var args = new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName);
        await minioClient.RemoveObjectAsync(args);
    }

    public string CreateFileName(string propertyName, string userId)
    {
        return $"{propertyName}{userId}";
    }

    private async Task EnsureBucket(string bucketName)
    {
        if (!await minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName)))
            await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName)).ConfigureAwait(false);
    }
}
