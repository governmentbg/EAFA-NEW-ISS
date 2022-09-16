using System.Collections.Generic;
using IARA.DomainModels.DTOModels.FishingAssociations;

namespace IARA.Interfaces
{
    public interface IFishingAssociationService
    {
        IEnumerable<FishingAssociationDTO> GetAllApprovedFishingAssociations();
    }
}
