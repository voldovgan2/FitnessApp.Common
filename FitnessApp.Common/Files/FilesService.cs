using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Common.Files
{
    public class FilesService : IFilesService
    {
        public FilesService()
        {
        }

        public Task UploadFile(string path, string name, Stream stream)
        {
            return Task.CompletedTask;
        }

        public Task<byte[]> DownloadFile(string path, string name)
        {
            byte[] result = Encoding.Default.GetBytes($"{path}_{name}");

            return Task.FromResult(result);
        }

        public Task DeleteFile(string path, string name)
        {
            return Task.CompletedTask;
        }

        public static string CreateBlobName(string propertyName, string userId)
        {
            return $"{propertyName}_{userId}";
        }
    }
}
