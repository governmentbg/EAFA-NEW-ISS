using System.Threading.Tasks;

namespace IARA.Mobile.Application.Interfaces.Utilities
{
    public interface IDownloader
    {
        Task<bool> DownloadFile(string fileName, string contentType, string url, object parameters = null);
    }
}
