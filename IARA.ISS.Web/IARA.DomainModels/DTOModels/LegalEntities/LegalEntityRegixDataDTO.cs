using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.LegalEntities
{
    public class LegalEntityRegixDataDTO : LegalEntityBaseRegixDataDTO
    {
        public List<AuthorizedPersonRegixDataDTO> AuthorizedPeople { get; set; }
    }
}
