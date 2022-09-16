using System.Collections.Generic;
using IARA.DomainModels.DTOModels.RecreationalFishing;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Interfaces.Nomenclatures
{
    public interface IRecreationalFishingNomenclaturesService
    {
        List<NomenclatureDTO> GetTicketPeriods();

        List<NomenclatureDTO> GetTicketTypes();

        List<RecreationalFishingTicketPriceDTO> GetTicketPrices();

        List<NomenclatureDTO> GetAllFishingAssociations();
    }
}
