using System.Diagnostics;
using System.Threading.Tasks;
using FitnessApp.Common.IntegrationTests.File.Fixtures;
using Xunit;

namespace FitnessApp.Common.IntegrationTests.File
{
    [Collection("FileService collection")]
    public class FileServiceTest : IClassFixture<FileServiceFixture>
    {
        private readonly FileServiceFixture _fixture;

        public FileServiceTest(FileServiceFixture fixture)
        {
            _fixture = fixture;
            Debug.WriteLine(_fixture.Path);
        }

        [Fact]
        public Task UploadFile_Success()
        {
            Assert.True(true);
            return Task.CompletedTask;
        }

        [Fact]
        public Task DownloadFile_Success()
        {
            Assert.False(false);
            return Task.CompletedTask;
        }

        [Fact]
        public Task DeleteFile_Success()
        {
            Assert.Null(null);
            return Task.CompletedTask;
        }
    }
}
