using System.Collections.Generic;
using IARA.Security.Permissions;

namespace IARA.Security
{
    public interface IPermissionsService
    {
        List<PermissionNomeclature> GetAllPermissions();

        List<string> GetPermissionNamesByIds(List<int> permissionIds);

        bool HasUserPermissions(int userId, params string[] permissions);

        bool HasUserPermissionsAny(int userId, params string[] permissions);

        List<int> GetUserPermissionIds(int userId);
    }
}
