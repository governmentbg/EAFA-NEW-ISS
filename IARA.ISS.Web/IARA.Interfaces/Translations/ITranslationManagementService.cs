using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.Translations;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces
{
    public interface ITranslationManagementService : IService
    {
        IQueryable<TranslationManagementDTO> GetAll(TranslationManagementFilters filters, bool helpers);

        TranslationManagementEditDTO Get(int id);

        TranslationManagementEditDTO GetByKey(string key);
        
        int AddEntry(TranslationManagementEditDTO resource);

        void EditEntry(TranslationManagementEditDTO resource);
        
        List<NomenclatureDTO> GetGroups();
    }
}
