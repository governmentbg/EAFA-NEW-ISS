using System.Collections.Generic;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Address
{
    public class AddressNomenclaturesDTO
    {
        public List<NomenclatureDTO> Countries { get; set; }
        public List<NomenclatureDTO> Districts { get; set; }
        public List<MunicipalityNomenclatureExtendedDTO> Municipalities { get; set; }
        public List<PopulatedAreaNomenclatureExtendedDTO> PopulatedAreas { get; set; }
    }
}
