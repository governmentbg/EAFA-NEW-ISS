using System.Linq;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using SixLabors.ImageSharp;

namespace IARA.Infrastructure.Services
{
    public class FileService : Service, IFileService
    {
        public FileService(IARADbContext db)
            : base(db)
        { }

        public DownloadableFileDTO GetFileForDownload(int id)
        {
            DownloadableFileDTO result = (from file in Db.Files
                                          where file.Id == id
                                          select new DownloadableFileDTO
                                          {
                                              Bytes = file.Content,
                                              MimeType = file.MimeType,
                                              FileName = file.Name
                                          }).Single();
            return result;
        }

        public DownloadableFileDTO GetResizedImage(int id, float maxWidth, float maxHeight, int compressionRate = 75)
        {
            DownloadableFileDTO result = (from file in Db.Files
                                          where file.Id == id & file.MimeType.Contains("image")
                                          select new DownloadableFileDTO
                                          {
                                              Bytes = file.Content,
                                              MimeType = file.MimeType,
                                              FileName = file.Name
                                          }).Single();

            Image image = Image.Load(result.Bytes);
            result.Bytes = image.Resize(maxWidth, maxHeight, compressionRate);
            return result;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.Files, id);
        }
    }
}
