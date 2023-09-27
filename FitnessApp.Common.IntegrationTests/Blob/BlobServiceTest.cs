using System.Threading.Tasks;
using FitnessApp.Common.IntegrationTests.Blob.Fixtures;
using FitnessApp.Comon.Tests.Shared;
using Xunit;

namespace FitnessApp.Common.IntegrationTests.Blob
{
    [Collection("BlobService collection")]
    public class BlobServiceTest : IClassFixture<BlobServiceFixture>
    {
        private readonly BlobServiceFixture _fixture;

        public BlobServiceTest(BlobServiceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task UploadFile_Success()
        {
            // Act
            await _fixture.BlobService.UploadFile(_fixture.Path, TestData.BlobToUpload, TestData.GetStream(TestData.BlobToUpload));
            var bolb = await _fixture.BlobService.DownloadFile(_fixture.Path, TestData.BlobToUpload);

            // Assert
            Assert.NotNull(bolb);
        }

        [Fact]
        public async Task DownloadFile_Success()
        {
            // Act
            var bolb = await _fixture.BlobService.DownloadFile(_fixture.Path, TestData.BlobToDownload);

            // Assert
            Assert.NotNull(bolb);
        }

        [Fact]
        public async Task DeleteFile_Success()
        {
            // Act
            await _fixture.BlobService.DeleteFile(_fixture.Path, TestData.BlobToDelete);
            var bolb = await _fixture.BlobService.DownloadFile(_fixture.Path, TestData.BlobToDelete);

            // Assert
            Assert.Null(bolb);
        }
    }
}
