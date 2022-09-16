using System.Linq;
using IARA.DomainModels.DTOModels.ApplicationRegiXChecks;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces
{
    public interface IApplicationRegiXChecksService : IService
    {
        IQueryable<ApplicationRegixCheckRequestDTO> GetAll(ApplicationRegiXChecksFilters filters);

        ApplicationRegixCheckRequestEditDTO Get(int id);
    }
}
