using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.PermissionsRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces
{
    public interface IPermissionsRegisterService : IService
    {
        /// <summary>
        /// Gets all permissions based on filters
        /// </summary>
        /// <param name="filters">The filters object</param>
        IQueryable<PermissionRegisterDTO> GetAllPermissions(PermissionsRegisterFilters filters);

        /// <summary>
        /// Get permission for edit dialog
        /// </summary>
        /// <param name="id">The id of the permission</param>
        PermissionRegisterEditDTO GetPermission(int id);

        /// <summary>
        /// Edits a permission (replaces entry with given DTO)
        /// </summary>
        /// <param name="permission">The DTO from the edit dialog</param>
        void EditPermission(PermissionRegisterEditDTO permission);

        /// <summary>
        /// Gets all permission types for dropdown
        /// </summary>
        List<NomenclatureDTO> GetAllPermissionTypes();

        /// <summary>
        /// Gets all permission groups for dropdown
        /// </summary>
        List<NomenclatureDTO> GetAllPermissionGroups();
    }
}
