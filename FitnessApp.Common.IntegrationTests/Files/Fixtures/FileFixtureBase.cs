using System;
using FitnessApp.Common.Files;
using Microsoft.Extensions.Configuration;
using Minio;

namespace FitnessApp.Common.IntegrationTests.File.Fixtures
{
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
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        { }
    }
}
