using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.NewsPublic;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces
{
    public interface INewsService
    {
        NewsDetailsDTO GetPublishedNews(int id);
        List<FileInfoDTO> GetNewsFiles(int id);
        IQueryable<NewsDTO> GetAll(NewsFilters filters);
        List<NewsImageDTO> GetNewsImages(int[] newsIds);
        IQueryable<NewsMobileDTO> GetAllMobileNews(NewsFilters filters);
        List<NewsMobileDTO> SetNewsHasImageFlag(List<NewsMobileDTO> newsList);
        DownloadableFileDTO GetNewsMainPhoto(int newsId);
        NewsDetailsMobileDTO GetNewsDetails(int newsId);
    }
}
