using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Common.Files;
using FitnessApp.Comon.Tests.Shared;
using Microsoft.Extensions.Configuration;
using Minio;

namespace FitnessApp.Common.IntegrationTests.File.Fixtures;

public class FileFixtureBase<T> : TestBase, IDisposable
    where T : class
{
    public FilesService FilesService { get; private set; }
    public string Path { get; private set; }
    public FileFixtureBase()
    {
        Path = typeof(T).Name.ToLower();
        var endpoint = Configuration.GetValue<string>("Minio:Endpoint");
        var accessKey = Configuration.GetValue<string>("Minio:AccessKey");
        var secretKey = Configuration.GetValue<string>("Minio:SecretKey");
        var secure = Configuration.GetValue<bool>("Minio:Secure");
        var minIoClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(secure)
            .Build();
        FilesService = new FilesService(minIoClient);

        var itemIds = new string[]
        {
            TestData.FileToDownload,
            TestData.FileToDelete,
            FilesService.CreateFileName(TestData.FileFieldName, TestData.EntityIdToGet),
            FilesService.CreateFileName(TestData.FileFieldName, TestData.EntityIdToDelete)
        };
        var createdItemsTasks = itemIds
            .Select(itemId => FilesService.UploadFile(
                Path.ToLower(),
                itemId,
                new MemoryStream(Encoding.Default.GetBytes(itemId))));
        Task.WhenAll(createdItemsTasks).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    { }
}
