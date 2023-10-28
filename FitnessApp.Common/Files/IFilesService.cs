using System.IO;
using System.Threading.Tasks;

namespace FitnessApp.Common.Files
{
    public interface IFilesService
    {
        Task UploadFile(string path, string name, Stream stream);
        Task<byte[]> DownloadFile(string path, string name);
        Task DeleteFile(string path, string name);
    }
}
