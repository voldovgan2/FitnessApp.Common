using System;
using System.Collections.Generic;
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
    public FilesService FileService { get; private set; }
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
        FileService = new FilesService(minIoClient);

        var itemIds = new string[]
        {
            TestData.FileToDownload,
            TestData.FileToDelete,
            FilesService.CreateFileName(TestData.FileFieldName, TestData.EntityIdToGet),
            FilesService.CreateFileName(TestData.FileFieldName, TestData.EntityIdToDelete)
        }
            .Concat(
                TestData
                    .CreateCollectionEntity(new Dictionary<string, object>
                    {
                        {
                            "Id", TestData.EntityIdToGet
                        },
                        {
                            "ItemsCount", 2
                        }
                    })
                    .Collection[TestData.CollectionName]
                    .Select(item => FilesService.CreateFileName(TestData.FileFieldName, item.Id))
            );
        var createdItemsTasks = itemIds
            .Select(itemId => FileService.UploadFile(
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
