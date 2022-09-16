using System.Linq;
using IARA.DomainModels.DTOModels.NewsManagment;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces
{
    public interface INewsManagementService : IService
    {
        IQueryable<NewsManagementDTO> GetAll(NewsManagmentFilters filters);

        NewsManagementEditDTO Get(int Id);

        int Add(NewsManagementEditDTO news);

        void Edit(NewsManagementEditDTO news);

        void DeleteNews(int Id);

        void UndoDeletedNews(int Id);

        string GetMainImage(int Id);

        void SendMobileNotifications();
    }
}
