using System.IO;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Common.IntegrationTests.File.Fixtures;
using FitnessApp.Comon.Tests.Shared;
using Xunit;

namespace FitnessApp.Common.IntegrationTests.File;

[Collection("FileService collection")]
public class FileServiceTest(FileServiceFixture fixture) : IClassFixture<FileServiceFixture>
{
    [Fact]
    public async Task UploadFile_Success()
    {
        await fixture.FilesService.UploadFile(fixture.Path.ToLower(), TestData.FileToUpload, new MemoryStream(Encoding.Default.GetBytes(TestData.FileToUpload)));
        Assert.True(true);
    }

    [Fact]
    public async Task DownloadFile_Success()
    {
        await fixture.FilesService.DownloadFile(fixture.Path.ToLower(), TestData.FileToDownload);
        Assert.True(true);
    }

    [Fact]
    public async Task DeleteFile_Success()
    {
        await fixture.FilesService.DeleteFile(fixture.Path.ToLower(), TestData.FileToDelete);
        Assert.True(true);
    }
}
