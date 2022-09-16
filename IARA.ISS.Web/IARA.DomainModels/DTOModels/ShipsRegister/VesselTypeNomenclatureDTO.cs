using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.ShipsRegister
{
    public class VesselTypeNomenclatureDTO : NomenclatureDTO
    {
        public int? ParentVesselTypeId { get; set; }
    }
}
