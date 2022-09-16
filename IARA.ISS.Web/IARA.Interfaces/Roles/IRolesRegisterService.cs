using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.RolesRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces
{
    public interface IRolesRegisterService : IService
    {
        /// <summary>
        /// Gets all roles based on filters
        /// </summary>
        /// <param name="filters">The filters object</param>
        IQueryable<RoleRegisterDTO> GetAllRoles(RolesRegisterFilters filters);

        /// <summary>
        /// Create a new role
        /// </summary>
        /// <param name="role">The role DTO from the edit dialog</param>
        /// <returns>The id of the newly created role</returns>
        int AddRole(RoleRegisterEditDTO role);

        /// <summary>
        /// Edits a role (replaces entry with given DTO(
        /// </summary>
        /// <param name="role">The DTO from the edit dialog</param>
        void EditRole(RoleRegisterEditDTO role);

        /// <summary>
        /// Soft deletes a role (sets IsActive = false)
        /// </summary>
        /// <param name="id">The id of the role</param>
        void DeleteRole(int id);

        /// <summary>
        /// Replaces a given role with another role for all users
        /// </summary>
        /// <param name="id">The id of the role</param>
        /// <param name="newRoleId">The id of the new role with which to replace</param>
        void DeleteAndReplaceRole(int id, int newRoleId);

        /// <summary>
        /// Soft restores a role (sets IsActive = true)
        /// </summary>
        /// <param name="id">The id of the role</param>
        void UndoDeleteRole(int id);

        /// <summary>
        /// Get role for edit dialog
        /// </summary>
        /// <param name="id">The id of the role</param>
        RoleRegisterEditDTO GetRole(int id);

        /// <summary>
        /// Gets all permission groups with all permissions for structured visualization
        /// </summary>
        List<PermissionGroupDTO> GetPermissionGroups();

        /// <summary>
        /// Gets all users that have a specific role
        /// </summary>
        /// <param name="roleId">The role id</param>
        List<NomenclatureDTO> GetUsersWithRole(int roleId);
    }
}
