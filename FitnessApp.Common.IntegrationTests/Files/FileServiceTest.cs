using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Common.IntegrationTests.File.Fixtures;
using FitnessApp.Comon.Tests.Shared;
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
        public async Task UploadFile_Success()
        {
            await _fixture.FileService.UploadFile(_fixture.Path.ToLower(), TestData.FileToUpload, new MemoryStream(Encoding.Default.GetBytes(TestData.FileToUpload)));
            Assert.True(true);
        }

        [Fact]
        public async Task DownloadFile_Success()
        {
            await _fixture.FileService.DownloadFile(_fixture.Path.ToLower(), TestData.FileToDownload);
            Assert.True(true);
        }

        [Fact]
        public async Task DeleteFile_Success()
        {
            await _fixture.FileService.DeleteFile(_fixture.Path.ToLower(), TestData.FileToDelete);
            Assert.True(true);
        }
    }
}
