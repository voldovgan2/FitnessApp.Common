using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using FitnessApp.Common.Blob;
using FitnessApp.Comon.Tests.Shared;
using Microsoft.Extensions.Configuration;

namespace FitnessApp.Common.IntegrationTests.Blob.Fixtures
{
    public class BlobFixtureBase<T> : TestBase, IDisposable
        where T : class
    {
        public BlobService BlobService { get; private set; }
        public string Path { get; private set; }

        private readonly BlobServiceClient _blobServiceClient;
        public BlobFixtureBase()
        {
            Path = typeof(T).Name.ToLower();
            var connectionString = Configuration.GetValue<string>("BlobSettings:ConnectionString");
            BlobService = new BlobService(connectionString);
            _blobServiceClient = new BlobServiceClient(connectionString);
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(Path);
            blobContainerClient.Create();
            var itemIds = new string[]
            {
                TestData.BlobToDownload,
                TestData.BlobToDelete,
                BlobService.CreateBlobName(TestData.BlobFieldName, TestData.EntityIdToGet),
                BlobService.CreateBlobName(TestData.BlobFieldName, TestData.EntityIdToDelete)
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
                        .Select(item => BlobService.CreateBlobName(TestData.BlobFieldName, item.Id))
                );
            var createdItemsTasks = itemIds.Select(itemId => blobContainerClient.GetBlobClient(itemId).UploadAsync(TestData.GetStream(itemId)));
            Task.WhenAll(createdItemsTasks).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _blobServiceClient?.GetBlobContainerClient(Path).DeleteAsync().GetAwaiter().GetResult();
            }
        }
    }
}
