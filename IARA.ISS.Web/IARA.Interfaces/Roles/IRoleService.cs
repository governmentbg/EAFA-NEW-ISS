using System.Collections.Generic;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Interfaces
{
    public interface IRoleService : IService
    {
        List<NomenclatureDTO> GetAllRoles();

        List<NomenclatureDTO> GetAllActiveRoles();

        List<NomenclatureDTO> GetInternalActiveRoles();

        List<NomenclatureDTO> GetPublicActiveRoles();
    }
}
