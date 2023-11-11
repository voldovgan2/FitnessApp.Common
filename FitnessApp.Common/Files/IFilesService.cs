using System.IO;
using System.Threading.Tasks;

namespace FitnessApp.Common.Files
{
    public interface IFilesService
    {
        Task UploadFile(string bucketName, string objectName, Stream stream);
        Task<byte[]> DownloadFile(string bucketName, string objectName);
        Task DeleteFile(string bucketName, string objectName);
    }
}
