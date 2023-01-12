using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Mobile.Pub.Application.DTObjects.News;

namespace IARA.Mobile.Pub.Application.Interfaces.Transactions
{
    public interface INewsTransaction
    {
        Task<List<NewsDto>> GetPagedNews(NewsFiltersDto filters, int pageSize, int pageNumber);
        Task<NewsDetailsDto> GetNewsDetail(int newsId);
    }
}
