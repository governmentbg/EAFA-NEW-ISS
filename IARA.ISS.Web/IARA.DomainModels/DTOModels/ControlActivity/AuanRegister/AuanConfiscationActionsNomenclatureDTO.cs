using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.ControlActivity.AuanRegister
{
    public class AuanConfiscationActionsNomenclatureDTO : NomenclatureDTO
    {
        public InspConfiscationActionGroupsEnum ActionGroup { get; set; }
    }
}
