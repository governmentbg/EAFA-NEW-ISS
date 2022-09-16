using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Common
{
    public class FishingGearNomenclatureDTO : NomenclatureDTO
    {
        public FishingGearParameterTypesEnum Type { get; set; }

        public bool IsForMutualFishing { get; set; }

        public bool HasHooks { get; set; }

        public HashSet<int> PermitLicenseIds { get; set; }
    }
}
