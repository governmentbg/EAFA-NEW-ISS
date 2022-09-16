using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.StatisticalForms.AquaFarms
{
    public class StatisticalFormAquacultureDTO
    {
        public int AquacultureId { get; set; }

        public string LegalName { get; set; }

        public string Eik { get; set; }

        public List<NomenclatureDTO> FacilityInstalations { get; set; }

        public List<NomenclatureDTO> FishTypes { get; set; }

        public AquacultureSystemEnum SystemType { get; set; }
    }
}
