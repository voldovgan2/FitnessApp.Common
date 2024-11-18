using System.IO;
using System.Threading.Tasks;
using FitnessApp.Common.Files;

namespace FitnessApp.Common.Tests.Fixtures;
public class MockFilesService : IFilesService
{
    public string CreateFileName(string propertyName, string userId)
    {
        return $"{propertyName}{userId}";
    }

    public Task DeleteFile(string bucketName, string objectName)
    {
        return Task.CompletedTask;
    }

    public Task<byte[]> DownloadFile(string bucketName, string objectName)
    {
        return Task.FromResult(new byte[0]);
    }

    public Task UploadFile(string bucketName, string objectName, Stream stream)
    {
        return Task.CompletedTask;
    }
}
