using System.IO;
using System.Threading.Tasks;

namespace FitnessApp.Common.Blob
{
    public interface IBlobService
    {
        Task UploadFile(string path, string name, Stream stream);
        Task<byte[]> DownloadFile(string path, string name);
        Task DeleteFile(string path, string name);
    }
}
