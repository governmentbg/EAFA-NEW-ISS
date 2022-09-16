using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Address
{
    public class PopulatedAreaNomenclatureExtendedDTO : NomenclatureDTO
    {
        public int MunicipalityId { get; set; }

        public char AreaType { get; set; }
    }
}
