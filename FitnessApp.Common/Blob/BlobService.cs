using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace FitnessApp.Common.Blob
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobService(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task UploadFile(string path, string name, Stream stream)
        {
            var blobClient = await EnsureBlobClientContainer(path, name);
            await blobClient.UploadAsync(stream);
        }

        public async Task<byte[]> DownloadFile(string path, string name)
        {
            var blobClient = await EnsureBlobClientContainer(path, name);
            var blobExists = await blobClient.ExistsAsync();
            byte[] result = null;
            if (blobExists)
            {
                var downloadResult = await blobClient.DownloadContentAsync();
                result = downloadResult.Value.Content.ToArray();
            }

            return result;
        }

        public async Task DeleteFile(string path, string name)
        {
            var blobClient = await EnsureBlobClientContainer(path, name);
            var blobExists = await blobClient.ExistsAsync();
            if (blobExists)
                await blobClient.DeleteAsync();
        }

        public static string CreateBlobName(string propertyName, string userId)
        {
            return $"{propertyName}_{userId}";
        }

        private async Task<BlobClient> EnsureBlobClientContainer(string path, string name)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(path);
            if (!await blobContainerClient.ExistsAsync())
            {
                blobContainerClient.Create();
            }

            var blobClient = blobContainerClient.GetBlobClient(name);
            return blobClient;
        }
    }
}
