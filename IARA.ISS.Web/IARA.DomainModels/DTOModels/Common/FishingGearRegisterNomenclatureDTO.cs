using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Common
{
    public class FishingGearRegisterNomenclatureDTO : NomenclatureDTO
    {
        public int FishingGearId { get; set; }

        public FishingGearParameterTypesEnum Type { get; set; }

        public bool IsForMutualFishing { get; set; }

        public bool HasHooks { get; set; }

        public int GearCount { get; set; }

        public int? HooksCount { get; set; }

        public decimal? NetEyeSize { get; set; }

        public HashSet<int> PermitLicenseIds { get; set; }

        public HashSet<int> InspectionIds { get; set; }
    }
}
