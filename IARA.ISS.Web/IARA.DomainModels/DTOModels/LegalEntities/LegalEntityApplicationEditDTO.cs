using System.Collections.Generic;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.LegalEntities
{
    public class LegalEntityApplicationEditDTO : LegalEntityBaseRegixDataDTO
    {
        public bool IsOnlineApplication { get; set; }

        public List<AuthorizedPersonDTO> AuthorizedPeople { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
