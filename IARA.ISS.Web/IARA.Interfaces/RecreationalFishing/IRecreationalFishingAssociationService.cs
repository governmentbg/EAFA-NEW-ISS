using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.RecreationalFishing;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces
{
    public interface IRecreationalFishingAssociationService : IService
    {
        IQueryable<RecreationalFishingAssociationDTO> GetAllAssociations(RecreationalFishingAssociationsFilters filters);

        RecreationalFishingAssociationEditDTO GetAssociation(int id);

        List<RecreationalFishingPossibleAssociationLegalDTO> GetPossibleAssociationLegals();

        RecreationalFishingAssociationEditDTO GetLegalForAssociation(int id);

        int AddAssociation(RecreationalFishingAssociationEditDTO association);

        void EditAssociation(RecreationalFishingAssociationEditDTO association);

        void DeleteAssociation(int id);

        void UndoDeleteAssociation(int id);

        List<NomenclatureDTO> GetUserFishingAssociations(int userId);

        List<NomenclatureDTO> GetAllAssociationNomenclatures();
    }
}
