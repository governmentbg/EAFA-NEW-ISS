using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.RolesRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services
{
    public class RolesRegisterService : Service, IRolesRegisterService
    {
        public RolesRegisterService(IARADbContext db)
            : base(db)
        { }

        public IQueryable<RoleRegisterDTO> GetAllRoles(RolesRegisterFilters filters)
        {
            IQueryable<RoleRegisterDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllRoles(showInactive);
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetParametersFilteredRoles(filters)
                    : GetFreeTextFilteredRoles(filters.FreeTextSearch, filters.ShowInactiveRecords);
            }
            return result;
        }

        public RoleRegisterEditDTO GetRole(int id)
        {
            RoleRegisterEditDTO role = (from r in Db.Roles
                                        where r.Id == id
                                        select new RoleRegisterEditDTO
                                        {
                                            Id = r.Id,
                                            Code = r.Code,
                                            Name = r.Name,
                                            Description = r.Description,
                                            ValidFrom = r.ValidFrom,
                                            ValidTo = r.ValidTo,
                                            HasInternalAccess = r.HasInternalAccess,
                                            HasPublicAccess = r.HasPublicAccess
                                        }).First();

            role.Users = GetRoleUsers(id);
            role.PermissionIds = GetRolePermissionIds(id);

            return role;
        }

        public int AddRole(RoleRegisterEditDTO role)
        {
            Role newRole = new Role
            {
                Code = role.Code,
                Name = role.Name,
                Description = role.Description,
                ValidFrom = role.ValidFrom.Value,
                ValidTo = role.ValidTo.Value,
                HasInternalAccess = role.HasInternalAccess.Value,
                HasPublicAccess = role.HasPublicAccess.Value,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            Db.Roles.Add(newRole);

            AddRolePermissions(newRole, role.PermissionIds);
            AddRoleUsers(newRole, role.Users);

            Db.SaveChanges();
            return newRole.Id;
        }

        public void EditRole(RoleRegisterEditDTO role)
        {
            Role dbRole = (from r in Db.Roles
                           where r.Id == role.Id.Value
                           select r).First();

            dbRole.Name = role.Name;
            dbRole.Description = role.Description;
            dbRole.ValidFrom = role.ValidFrom.Value;
            dbRole.ValidTo = role.ValidTo.Value;
            dbRole.HasInternalAccess = role.HasInternalAccess.Value;
            dbRole.HasPublicAccess = role.HasPublicAccess.Value;

            AddOrEditRolePermissions(dbRole.Id, role.PermissionIds);
            AddOrEditRoleUsers(dbRole.Id, role.Users);

            Db.SaveChanges();
        }

        public void DeleteRole(int id)
        {
            Role role = Db.Roles.Where(x => x.Id == id).First();
            role.ValidTo = DateTime.Now;
            Db.SaveChanges();
        }

        public void DeleteAndReplaceRole(int id, int newRoleId)
        {
            DateTime now = DateTime.Now;

            Role dbRole = (from role in Db.Roles
                           where role.Id == id
                           select role).First();

            dbRole.ValidTo = now;

            // invalidate all role for all users
            List<UserRole> dbUserRoles = (from userRole in dbRole.UserRoles
                                          where userRole.RoleId == id
                                               && userRole.IsActive
                                               && userRole.AccessValidFrom <= now
                                               && userRole.AccessValidTo > now
                                          select userRole).ToList();

            foreach (UserRole userRole in dbUserRoles)
            {
                userRole.AccessValidTo = now;
            }

            // set new role for all users
            List<int> userIds = dbUserRoles.Where(x => x.User.ValidFrom <= now && x.User.ValidTo > now).Select(x => x.UserId).ToList();
            foreach (int userId in userIds)
            {
                bool exists = (from userRole in Db.UserRoles
                               where userRole.UserId == userId
                                    && userRole.RoleId == newRoleId
                                    && userRole.IsActive
                                    && userRole.AccessValidFrom <= now
                                    && userRole.AccessValidTo > now
                               select userRole).Any();

                if (!exists)
                {
                    UserRole entry = new UserRole
                    {
                        RoleId = newRoleId,
                        UserId = userId,
                        AccessValidFrom = now,
                        AccessValidTo = DefaultConstants.MAX_VALID_DATE
                    };

                    Db.UserRoles.Add(entry);
                }
            }

            Db.SaveChanges();
        }

        public void UndoDeleteRole(int id)
        {
            Role role = Db.Roles.Where(x => x.Id == id).First();
            role.ValidTo = DefaultConstants.MAX_VALID_DATE;
            Db.SaveChanges();
        }

        public List<PermissionGroupDTO> GetPermissionGroups()
        {
            List<PermissionGroupDTO> groups = (from permGroup in Db.NpermissionGroups
                                               join parentGroup in Db.NpermissionGroups on permGroup.ParentGroupId equals parentGroup.Id into pG
                                               from parentGroup in pG.DefaultIfEmpty()
                                               orderby permGroup.OrderNo
                                               select new PermissionGroupDTO
                                               {
                                                   Id = permGroup.Id,
                                                   Name = permGroup.Name,
                                                   ParentGroup = parentGroup != null ? parentGroup.Name : null
                                               }).ToList();

            var groupPerms = (from permission in Db.Npermissions
                              join permissionType in Db.NpermissionTypes on permission.PermissionTypeId equals permissionType.Id
                              orderby permission.OrderNo
                              select new
                              {
                                  GroupID = permission.PermissionGroupId,
                                  Permission = new NomenclatureDTO
                                  {
                                      Value = permission.Id,
                                      DisplayName = permission.Name,
                                      Description = permissionType.Code == nameof(PermissionTypeEnum.OTHER) ? permission.Description : null,
                                      Code = permissionType.Code
                                  }
                              }).ToLookup(x => x.GroupID, y => y.Permission);

            foreach (PermissionGroupDTO group in groups)
            {
                List<NomenclatureDTO> permissions = groupPerms[group.Id].ToList();
                group.ReadAllPermission = permissions.Where(x => x.Code == nameof(PermissionTypeEnum.READ_ALL)).SingleOrDefault();
                group.ReadPermission = permissions.Where(x => x.Code == nameof(PermissionTypeEnum.READ)).SingleOrDefault();
                group.AddPermission = permissions.Where(x => x.Code == nameof(PermissionTypeEnum.ADD)).SingleOrDefault();
                group.EditPermission = permissions.Where(x => x.Code == nameof(PermissionTypeEnum.EDIT)).SingleOrDefault();
                group.DeletePermission = permissions.Where(x => x.Code == nameof(PermissionTypeEnum.DELETE)).SingleOrDefault();
                group.RestorePermission = permissions.Where(x => x.Code == nameof(PermissionTypeEnum.RESTORE)).SingleOrDefault();
                group.OtherPermissions = permissions.Where(x => x.Code == nameof(PermissionTypeEnum.OTHER)).ToList();
            }
            return groups;
        }

        public List<NomenclatureDTO> GetUsersWithRole(int roleId)
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> users = (from userRole in Db.UserRoles
                                           join user in Db.Users on userRole.UserId equals user.Id
                                           join person in Db.Persons on user.PersonId equals person.Id
                                           where userRole.RoleId == roleId
                                               && userRole.IsActive
                                               && userRole.AccessValidFrom <= now
                                               && userRole.AccessValidTo > now
                                               && user.ValidFrom <= now
                                               && user.ValidTo > now
                                           select new NomenclatureDTO
                                           {
                                               Value = user.Id,
                                               DisplayName = $"{person.FirstName} {person.LastName} ({user.Username})"
                                           }).ToList();

            return users;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.Roles, id);
            return audit;
        }

        private IQueryable<RoleRegisterDTO> GetAllRoles(bool showInactive)
        {
            DateTime now = DateTime.Now;

            IQueryable<RoleRegisterDTO> query = from role in Db.Roles
                                                orderby role.Code, role.Name
                                                select new RoleRegisterDTO
                                                {
                                                    Id = role.Id,
                                                    Code = role.Code,
                                                    Name = role.Name,
                                                    Description = role.Description,
                                                    ValidFrom = role.ValidFrom,
                                                    ValidTo = role.ValidTo,
                                                    UsersCount = role.UserRoles.Count(x => x.IsActive
                                                                                    && x.AccessValidFrom <= now
                                                                                    && x.AccessValidTo > now
                                                                                    && x.User.ValidFrom <= now
                                                                                    && x.User.ValidTo > now),
                                                    HasInternalAccess = role.HasInternalAccess,
                                                    HasPublicAccess = role.HasPublicAccess,
                                                    IsActive = role.ValidFrom <= now && role.ValidTo > now
                                                };

            query = query.Where(x => x.IsActive == !showInactive);
            return query;
        }

        private IQueryable<RoleRegisterDTO> GetParametersFilteredRoles(RolesRegisterFilters filters)
        {
            DateTime now = DateTime.Now;

            IQueryable<RoleRegisterDTO> query = from role in Db.Roles
                                                orderby role.Code, role.Name
                                                select new RoleRegisterDTO
                                                {
                                                    Id = role.Id,
                                                    Code = role.Code,
                                                    Name = role.Name,
                                                    Description = role.Description,
                                                    ValidFrom = role.ValidFrom,
                                                    ValidTo = role.ValidTo,
                                                    UsersCount = role.UserRoles.Count(x => x.IsActive
                                                                                    && x.AccessValidFrom <= now
                                                                                    && x.AccessValidTo > now
                                                                                    && x.User.ValidFrom <= now
                                                                                    && x.User.ValidTo > now),
                                                    HasInternalAccess = role.HasInternalAccess,
                                                    HasPublicAccess = role.HasPublicAccess,
                                                    IsActive = role.ValidFrom <= now && role.ValidTo > now
                                                };

            query = query.Where(x => x.IsActive == !filters.ShowInactiveRecords);

            if (!string.IsNullOrEmpty(filters.Code))
            {
                query = query.Where(x => x.Code.ToLower().Contains(filters.Code.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.Name))
            {
                query = query.Where(x => x.Name.ToLower().Contains(filters.Name.ToLower()));
            }

            if (filters.PermissionId.HasValue)
            {
                query = from role in query
                        join rolePerm in Db.RolePermissions on role.Id equals rolePerm.RoleId
                        where rolePerm.PermissionId == filters.PermissionId.Value
                        select role;
            }

            if (filters.ValidFrom.HasValue)
            {
                query = query.Where(x => x.ValidFrom >= filters.ValidFrom);
            }

            if (filters.ValidTo.HasValue)
            {
                query = query.Where(x => x.ValidTo <= filters.ValidTo);
            }

            return query;
        }

        private IQueryable<RoleRegisterDTO> GetFreeTextFilteredRoles(string text, bool showInactive)
        {
            text = text.ToLowerInvariant();

            DateTime now = DateTime.Now;
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);

            IQueryable<RoleRegisterDTO> query = from role in Db.Roles
                                                where role.Code.ToLower().Contains(text)
                                                    || role.Name.ToLower().Contains(text)
                                                    || role.Description.ToLower().Contains(text)
                                                    || (searchDate.HasValue && (role.ValidFrom == searchDate || role.ValidTo == searchDate))
                                                orderby role.Code, role.Name
                                                select new RoleRegisterDTO
                                                {
                                                    Id = role.Id,
                                                    Code = role.Code,
                                                    Name = role.Name,
                                                    Description = role.Description,
                                                    ValidFrom = role.ValidFrom,
                                                    ValidTo = role.ValidTo,
                                                    UsersCount = role.UserRoles.Count(x => x.IsActive
                                                                                    && x.AccessValidFrom <= now
                                                                                    && x.AccessValidTo > now
                                                                                    && x.User.ValidFrom <= now
                                                                                    && x.User.ValidTo > now),
                                                    HasInternalAccess = role.HasInternalAccess,
                                                    HasPublicAccess = role.HasPublicAccess,
                                                    IsActive = role.ValidFrom <= now && role.ValidTo > now
                                                };

            query = query.Where(x => x.IsActive == !showInactive);
            return query;
        }

        private List<UserRoleRegisterDTO> GetRoleUsers(int roleId)
        {
            DateTime now = DateTime.Now;

            List<UserRoleRegisterDTO> users = (from userRole in Db.UserRoles
                                               join user in Db.Users on userRole.UserId equals user.Id
                                               where userRole.RoleId == roleId
                                                    && user.ValidFrom <= now
                                                    && user.ValidTo > now
                                               select new UserRoleRegisterDTO
                                               {
                                                   Id = userRole.Id,
                                                   RoleId = userRole.RoleId,
                                                   UserId = user.Id,
                                                   AccessValidFrom = userRole.AccessValidFrom,
                                                   AccessValidTo = userRole.AccessValidTo,
                                                   IsActive = userRole.IsActive
                                               }).ToList();

            return users;
        }

        private List<int> GetRolePermissionIds(int roleId)
        {
            List<int> result = (from rolePerm in Db.RolePermissions
                                where rolePerm.RoleId == roleId
                                    && rolePerm.IsActive
                                select rolePerm.PermissionId).ToList();

            return result;
        }

        private void AddRolePermissions(Role role, List<int> permissionIds)
        {
            if (permissionIds != null)
            {
                foreach (int permissionId in permissionIds)
                {
                    RolePermission entry = new RolePermission
                    {
                        Role = role,
                        PermissionId = permissionId
                    };

                    Db.RolePermissions.Add(entry);
                }
            }
        }

        private void AddOrEditRolePermissions(int roleId, List<int> permissionsIds)
        {
            List<RolePermission> rolePermissions = (from rp in Db.RolePermissions
                                                    where rp.RoleId == roleId
                                                    select rp).ToList();

            if (permissionsIds == null)
            {
                foreach (RolePermission rolePerm in rolePermissions)
                {
                    rolePerm.IsActive = false;
                }
            }
            else
            {
                List<int> currentPermissionIds = rolePermissions.Where(x => x.IsActive).Select(x => x.PermissionId).ToList();
                List<int> permissionIdsToAdd = permissionsIds.Where(x => !currentPermissionIds.Contains(x)).ToList();
                List<int> permissionIdsToRemove = currentPermissionIds.Where(x => !permissionsIds.Contains(x)).ToList();

                foreach (int permissionId in permissionIdsToAdd)
                {
                    RolePermission dbRolePerm = rolePermissions.Where(x => x.PermissionId == permissionId).SingleOrDefault();

                    if (dbRolePerm != null)
                    {
                        dbRolePerm.IsActive = true;
                    }
                    else
                    {
                        RolePermission entry = new RolePermission
                        {
                            RoleId = roleId,
                            PermissionId = permissionId
                        };

                        Db.RolePermissions.Add(entry);
                    }
                }

                List<RolePermission> rolePermsToRemove = rolePermissions.Where(x => permissionIdsToRemove.Contains(x.PermissionId)).ToList();
                foreach (RolePermission dbRolePerm in rolePermsToRemove)
                {
                    dbRolePerm.IsActive = false;
                }
            }
        }

        private void AddRoleUsers(Role role, List<UserRoleRegisterDTO> users)
        {
            if (users != null)
            {
                foreach (UserRoleRegisterDTO user in users)
                {
                    UserRole userRole = new UserRole
                    {
                        Role = role,
                        UserId = user.UserId.Value,
                        AccessValidFrom = user.AccessValidFrom.Value,
                        AccessValidTo = user.AccessValidTo.Value,
                        IsActive = user.IsActive.Value
                    };

                    Db.UserRoles.Add(userRole);
                }
            }
        }

        private void AddOrEditRoleUsers(int roleId, List<UserRoleRegisterDTO> users)
        {
            List<UserRole> userRoles = (from ur in Db.UserRoles
                                        where ur.RoleId == roleId
                                        select ur).ToList();

            if (users == null)
            {
                foreach (UserRole userRole in userRoles)
                {
                    userRole.IsActive = false;
                }
            }
            else
            {
                foreach (UserRoleRegisterDTO user in users)
                {
                    if (user.Id.HasValue)
                    {
                        UserRole dbUserRole = userRoles.Where(x => x.Id == user.Id.Value).Single();

                        dbUserRole.UserId = user.UserId.Value;
                        dbUserRole.AccessValidFrom = user.AccessValidFrom.Value;
                        dbUserRole.AccessValidTo = user.AccessValidTo.Value;
                        dbUserRole.IsActive = user.IsActive.Value;
                    }
                    else
                    {
                        UserRole newUserRole = new UserRole
                        {
                            RoleId = roleId,
                            UserId = user.UserId.Value,
                            AccessValidFrom = user.AccessValidFrom.Value,
                            AccessValidTo = user.AccessValidTo.Value,
                            IsActive = user.IsActive.Value
                        };

                        Db.UserRoles.Add(newUserRole);
                    }
                }
            }
        }
    }
}
