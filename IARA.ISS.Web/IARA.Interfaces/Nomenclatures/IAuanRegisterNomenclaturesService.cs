using System.Collections.Generic;
using IARA.DomainModels.DTOModels.ControlActivity.AuanRegister;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Interfaces.Nomenclatures
{
    public interface IAuanRegisterNomenclaturesService
    {
        List<NomenclatureDTO> GetAllInspectionReports();

        List<AuanConfiscationActionsNomenclatureDTO> GetConfiscationActions();

        List<InspDeliveryTypesNomenclatureDTO> GetInspDeliveryTypes();

        List<InspDeliveryTypesNomenclatureDTO> GetInspDeliveryConfirmationTypes();

        List<NomenclatureDTO> GetAuanStatuses();

        List<NomenclatureDTO> GetConfiscatedAppliances();
    }
}
