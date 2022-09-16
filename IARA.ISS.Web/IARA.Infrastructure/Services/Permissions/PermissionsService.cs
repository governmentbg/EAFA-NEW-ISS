using System;
using System.Collections.Generic;
using System.Linq;
using IARA.DataAccess;
using IARA.Security;
using IARA.Security.Permissions;

namespace IARA.Infrastructure.Services
{
    public class PermissionsService : BaseService, IPermissionsService
    {
        public PermissionsService(IARADbContext db)
             : base(db)
        { }

        public List<PermissionNomeclature> GetAllPermissions()
        {
            DateTime now = DateTime.Now;
            return Db.Npermissions
                .Where(x => x.ValidFrom < now && x.ValidTo > now)
                .Select(x => new PermissionNomeclature
                {
                    ID = x.Id,
                    Name = x.Name
                }).ToList();
        }

        public List<string> GetPermissionNamesByIds(List<int> permissionIds)
        {
            DateTime now = DateTime.Now;

            return Db.Npermissions.Where(x => permissionIds.Contains(x.Id)).Select(x => x.Name).ToList();
        }

        public bool HasUserPermissions(int userId, params string[] permissions)
        {
            List<int> userPermIds = GetUserPermissionIds(userId);
            List<int> permIds = GetPermissionIdsByNames(permissions);

            return permIds.All(id => userPermIds.Contains(id));
        }

        public bool HasUserPermissionsAny(int userId, params string[] permissions)
        {
            List<int> userPermIds = GetUserPermissionIds(userId);
            List<int> permIds = GetPermissionIdsByNames(permissions);

            return permIds.Any(id => userPermIds.Contains(id));
        }

        public List<int> GetUserPermissionIds(int userId)
        {
            List<int> ids = (from userRole in Db.UserRoles
                             join role in Db.Roles on userRole.RoleId equals role.Id
                             join rolePermission in Db.RolePermissions on role.Id equals rolePermission.RoleId
                             join permission in Db.Npermissions on rolePermission.PermissionId equals permission.Id
                             where userRole.UserId == userId
                                 && userRole.IsActive
                                 && rolePermission.IsActive
                             select permission.Id).ToList();

            List<int> legalPermIds = (from userLegal in Db.UserLegals
                                      join role in Db.Roles on userLegal.RoleId equals role.Id
                                      join rolePermission in Db.RolePermissions on role.Id equals rolePermission.RoleId
                                      join permission in Db.Npermissions on rolePermission.PermissionId equals permission.Id
                                      where userLegal.UserId == userId
                                            && userLegal.IsActive
                                            && rolePermission.IsActive
                                      select permission.Id).ToList();

            return ids.Concat(legalPermIds).Distinct().ToList();
        }

        private List<int> GetPermissionIdsByNames(params string[] permissions)
        {
            List<int> ids = (from perm in Db.Npermissions
                             where permissions.Contains(perm.Name)
                             select perm.Id).ToList();

            return ids;
        }
    }
}
