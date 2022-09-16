using System;
using System.Collections.Generic;
using System.Linq;
using IARA.DataAccess;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services
{
    public class RoleService : Service, IRoleService
    {
        public RoleService(IARADbContext db)
            : base(db)
        { }

        public List<NomenclatureDTO> GetAllRoles()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> roles = (from role in Db.Roles
                                           orderby role.Name
                                           select new NomenclatureDTO
                                           {
                                               Value = role.Id,
                                               DisplayName = role.Name,
                                               IsActive = role.ValidFrom <= now && role.ValidTo > now
                                           }).ToList();

            return roles;
        }

        public List<NomenclatureDTO> GetAllActiveRoles()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> roles = (from role in Db.Roles
                                           where role.ValidFrom <= now && role.ValidTo >= now
                                           orderby role.Name
                                           select new NomenclatureDTO
                                           {
                                               Value = role.Id,
                                               DisplayName = role.Name
                                           }).ToList();
            return roles;
        }

        public List<NomenclatureDTO> GetInternalActiveRoles()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> roles = (from role in Db.Roles
                                           where role.HasInternalAccess
                                               && role.ValidFrom <= now
                                               && role.ValidTo >= now
                                           orderby role.Name
                                           select new NomenclatureDTO
                                           {
                                               Value = role.Id,
                                               DisplayName = role.Name
                                           }).ToList();
            return roles;
        }

        public List<NomenclatureDTO> GetPublicActiveRoles()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> roles = (from role in Db.Roles
                                           where role.HasPublicAccess
                                               && role.ValidFrom <= now
                                               && role.ValidTo >= now
                                           orderby role.Name
                                           select new NomenclatureDTO
                                           {
                                               Value = role.Id,
                                               DisplayName = role.Name
                                           }).ToList();
            return roles;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return base.GetSimpleEntityAuditValues(Db.Users, id);
        }
    }
}
