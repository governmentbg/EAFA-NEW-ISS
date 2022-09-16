using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.StatisticalForms.AquaFarms
{
    public class StatisticalFormAquacultureNomenclatureDTO : NomenclatureDTO
    {
        public string UrorNum { get; set; }

        public string RegNum { get; set; }

        public string EIK { get; set; }

        public string LegalName { get; set; }
    }
}
