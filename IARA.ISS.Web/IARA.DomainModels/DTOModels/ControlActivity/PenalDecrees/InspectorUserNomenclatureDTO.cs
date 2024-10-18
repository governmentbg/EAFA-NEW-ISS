using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.ControlActivity.PenalDecrees
{
    public class InspectorUserNomenclatureDTO : NomenclatureDTO
    {
        public string IssuerPosition { get; set; }

        public int? SectorId { get; set; }

        public int? DepartmentId { get; set; }

        public string DepartmentAddress { get; set; }
    }
}
