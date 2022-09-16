using System;
using System.Collections.Generic;
using System.Linq;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.PermissionsRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services
{
    public class PermissionsRegisterService : Service, IPermissionsRegisterService
    {
        public PermissionsRegisterService(IARADbContext db)
            : base(db)
        { }

        public IQueryable<PermissionRegisterDTO> GetAllPermissions(PermissionsRegisterFilters filters)
        {
            IQueryable<PermissionRegisterDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetAllPermissions();
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetParametersFilteredPermissions(filters)
                    : GetFreeTextFilteredPermissions(filters.FreeTextSearch);
            }
            return result;
        }

        public PermissionRegisterEditDTO GetPermission(int id)
        {
            PermissionRegisterEditDTO result = (from permission in Db.Npermissions
                                                where permission.Id == id
                                                select new PermissionRegisterEditDTO
                                                {
                                                    Id = permission.Id,
                                                    Name = permission.Name,
                                                    Description = permission.Description,
                                                    GroupId = permission.PermissionGroupId,
                                                    TypeId = permission.PermissionTypeId
                                                }).First();

            result.Roles = GetRolePermissions(id);
            return result;
        }

        public void EditPermission(PermissionRegisterEditDTO permission)
        {
            Npermission dbPermission = (from perm in Db.Npermissions
                                            .AsSplitQuery()
                                            .Include(x => x.RolePermissions)
                                        where perm.Id == permission.Id
                                        select perm).First();

            dbPermission.Description = permission.Description;

            List<int> roleIds = permission.Roles != null ? permission.Roles.Select(x => x.RoleId.Value).ToList() : null;
            AddOrEditPermissionRoles(dbPermission, roleIds);

            Db.SaveChanges();
        }

        public List<NomenclatureDTO> GetAllPermissionTypes()
        {
            List<NomenclatureDTO> types = (from type in Db.NpermissionTypes
                                           orderby type.Name
                                           select new NomenclatureDTO
                                           {
                                               Value = type.Id,
                                               DisplayName = type.Name,
                                               IsActive = true
                                           }).ToList();
            return types;
        }

        public List<NomenclatureDTO> GetAllPermissionGroups()
        {
            List<NomenclatureDTO> groups = (from gr in Db.NpermissionGroups
                                            orderby gr.Name
                                            select new NomenclatureDTO
                                            {
                                                Value = gr.Id,
                                                DisplayName = gr.Name,
                                                IsActive = true
                                            }).ToList();
            return groups;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.Npermissions, id);
        }

        /// <summary>
        /// Gets all permissions
        /// </summary>
        private IQueryable<PermissionRegisterDTO> GetAllPermissions()
        {
            DateTime now = DateTime.Now;

            IQueryable<PermissionRegisterDTO> permissions = from permission in Db.Npermissions
                                                            join type in Db.NpermissionTypes on permission.PermissionTypeId equals type.Id
                                                            join gr in Db.NpermissionGroups on permission.PermissionGroupId equals gr.Id
                                                            where permission.ValidFrom <= now && permission.ValidTo > now
                                                            orderby permission.OrderNo, permission.Name
                                                            select new PermissionRegisterDTO
                                                            {
                                                                Id = permission.Id,
                                                                Name = permission.Name,
                                                                Description = permission.Description,
                                                                Type = type.Name,
                                                                Group = gr.Name,
                                                                RolesCount = permission.RolePermissions.Count(x => x.IsActive)
                                                            };
            return permissions;
        }

        /// <summary>
        /// Gets all permissions based on complex filters
        /// </summary>
        /// <param name="filters">The filters object</param>
        private IQueryable<PermissionRegisterDTO> GetParametersFilteredPermissions(PermissionsRegisterFilters filters)
        {
            DateTime now = DateTime.Now;

            var query = from permission in Db.Npermissions
                        join type in Db.NpermissionTypes on permission.PermissionTypeId equals type.Id
                        join gr in Db.NpermissionGroups on permission.PermissionGroupId equals gr.Id
                        where permission.ValidFrom <= now && permission.ValidTo > now
                        select new
                        {
                            permission.Id,
                            permission.Name,
                            permission.OrderNo,
                            permission.Description,
                            permission.PermissionTypeId,
                            PermissionTypeName = type.Name,
                            permission.PermissionGroupId,
                            PermissionGroupName = gr.Name,
                            RolesCount = permission.RolePermissions.Count(x => x.IsActive)
                        };

            if (!string.IsNullOrEmpty(filters.Name))
            {
                query = query.Where(x => x.Name.ToLower().Contains(filters.Name.ToLower()));
            }

            if (filters.GroupId.HasValue)
            {
                query = query.Where(x => x.PermissionGroupId == filters.GroupId.Value);
            }

            if (filters.TypeIds != null && filters.TypeIds.Count != 0)
            {
                query = query.Where(x => filters.TypeIds.Contains(x.PermissionTypeId));
            }

            if (filters.RoleId.HasValue)
            {
                query = from permission in query
                        join rolePerm in Db.RolePermissions on permission.Id equals rolePerm.PermissionId
                        where rolePerm.RoleId == filters.RoleId.Value
                            && rolePerm.IsActive
                        select permission;
            }

            IQueryable<PermissionRegisterDTO> permissions = from permission in query
                                                            orderby permission.OrderNo, permission.Name
                                                            select new PermissionRegisterDTO
                                                            {
                                                                Id = permission.Id,
                                                                Name = permission.Name,
                                                                Description = permission.Description,
                                                                Group = permission.PermissionGroupName,
                                                                Type = permission.PermissionTypeName,
                                                                RolesCount = permission.RolesCount
                                                            };
            return permissions;
        }

        /// <summary>
        /// Gets all permissions based on a free-text filter
        /// </summary>
        /// <param name="text">The text by which to filter</param>
        private IQueryable<PermissionRegisterDTO> GetFreeTextFilteredPermissions(string text)
        {
            DateTime now = DateTime.Now;
            text = text.ToLowerInvariant();

            IQueryable<PermissionRegisterDTO> permissions = from permission in Db.Npermissions
                                                            join type in Db.NpermissionTypes on permission.PermissionTypeId equals type.Id
                                                            join gr in Db.NpermissionGroups on permission.PermissionGroupId equals gr.Id
                                                            where permission.ValidFrom <= now && permission.ValidTo > now
                                                                && (permission.Name.ToLower().Contains(text)
                                                                || permission.Description.ToLower().Contains(text)
                                                                || gr.Name.ToLower().Contains(text)
                                                                || type.Name.ToLower().Contains(text))
                                                            orderby permission.OrderNo, permission.Name
                                                            select new PermissionRegisterDTO
                                                            {
                                                                Id = permission.Id,
                                                                Name = permission.Name,
                                                                Description = permission.Description,
                                                                Group = gr.Name,
                                                                Type = type.Name,
                                                                RolesCount = permission.RolePermissions.Count(x => x.IsActive)
                                                            };
            return permissions;
        }

        /// <summary>
        /// Gets all roles for given permission
        /// </summary>
        /// <param name="permissionId">The id of the permission</param>
        private List<RolePermissionRegisterDTO> GetRolePermissions(int permissionId)
        {
            List<RolePermissionRegisterDTO> result = (from rolePerm in Db.RolePermissions
                                                      join role in Db.Roles on rolePerm.RoleId equals role.Id
                                                      where rolePerm.PermissionId == permissionId
                                                          && rolePerm.IsActive
                                                      orderby role.Name
                                                      select new RolePermissionRegisterDTO
                                                      {
                                                          PermissionId = rolePerm.PermissionId,
                                                          RoleId = role.Id,
                                                          IsActive = rolePerm.IsActive
                                                      }).ToList();

            return result;
        }

        /// <summary>
        /// Add or edits a role permission
        /// </summary>
        /// <param name="permission">The permission entity</param>
        /// <param name="roleIds">List of new role ids</param>
        private void AddOrEditPermissionRoles(Npermission permission, List<int> roleIds)
        {
            if (roleIds == null)
            {
                foreach (RolePermission rolePerm in permission.RolePermissions)
                {
                    rolePerm.IsActive = false;
                }
            }
            else
            {
                List<int> currentRoleIds = permission.RolePermissions.Where(x => x.IsActive).Select(x => x.RoleId).ToList();
                List<int> roleIdsToAdd = roleIds.Where(x => !currentRoleIds.Contains(x)).ToList();
                List<int> roleIdsToDelete = currentRoleIds.Where(x => !roleIds.Contains(x)).ToList();

                foreach (int roleId in roleIdsToAdd)
                {
                    RolePermission dbRolePerm = permission.RolePermissions.Where(x => x.RoleId == roleId).SingleOrDefault();
                    if (dbRolePerm != null)
                    {
                        dbRolePerm.IsActive = true;
                    }
                    else
                    {
                        RolePermission entry = new RolePermission
                        {
                            Permission = permission,
                            RoleId = roleId
                        };

                        Db.RolePermissions.Add(entry);
                    }
                }

                foreach (RolePermission dbRolePerm in permission.RolePermissions.Where(x => roleIdsToDelete.Contains(x.RoleId)))
                {
                    dbRolePerm.IsActive = false;
                }
            }
        }
    }
}
