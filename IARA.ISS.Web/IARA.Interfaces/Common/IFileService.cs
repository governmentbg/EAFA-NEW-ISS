using IARA.DomainModels.DTOModels.Files;

namespace IARA.Interfaces
{
    public interface IFileService
    {
        DownloadableFileDTO GetFileForDownload(int id);

        DownloadableFileDTO GetResizedImage(int id, float maxWidth, float maxHeight, int compressionRate = 75);
    }
}
