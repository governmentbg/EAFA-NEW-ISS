using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.DTObjects.News;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.Application.Transactions.Base;

namespace IARA.Mobile.Pub.Application.Transactions
{
    public class NewsTransaction : BaseTransaction, INewsTransaction
    {
        public NewsTransaction(BaseTransactionProvider provider) : base(provider)
        {
        }

        public async Task<List<NewsDto>> GetPagedNews(NewsFiltersDto filters, int pageSize, int pageNumber)
        {
            GridRequest<NewsFiltersDto> gridRequest = new GridRequest<NewsFiltersDto>(filters)
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
            };

            HttpResult<List<NewsDto>> result = await RestClient.PostAsync<List<NewsDto>>("News/GetAll", gridRequest);

            if (result.IsSuccessful && result.Content.Count > 0)
            {
                return result.Content;
            }

            return null;
        }

        public async Task<NewsDetailsDto> GetNewsDetail(int newsId)
        {
            HttpResult<NewsDetailsDto> result = await RestClient.GetAsync<NewsDetailsDto>("News/GetNewsDetails", new { newsId });

            if (result.IsSuccessful)
            {
                return result.Content;
            }

            return null;
        }
    }
}
