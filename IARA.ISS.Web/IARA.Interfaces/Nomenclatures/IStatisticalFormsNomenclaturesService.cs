using System.Collections.Generic;
using IARA.DomainModels.DTOModels.StatisticalForms.AquaFarms;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Interfaces.Nomenclatures
{
    public interface IStatisticalFormsNomenclaturesService
    {
        List<NomenclatureDTO> GetVesselLengthIntervals();
        List<NomenclatureDTO> GetGrossTonnageIntervals();
        List<NomenclatureDTO> GetFuelTypes();
        List<NomenclatureDTO> GetReworkProductTypes();
        List<StatisticalFormAquacultureNomenclatureDTO> GetAllAquacultureNomenclatures();
    }
}
