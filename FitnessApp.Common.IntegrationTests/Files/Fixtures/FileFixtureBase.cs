using System;
using FitnessApp.Common.Files;

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
            FileService = new FilesService();
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
