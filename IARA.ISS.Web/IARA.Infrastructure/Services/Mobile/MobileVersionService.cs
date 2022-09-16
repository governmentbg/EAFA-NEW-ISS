using System;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Mobile.Versions;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services
{
    public class MobileVersionService : Service, IMobileVersionService
    {
        public MobileVersionService(IARADbContext dbContext)
            : base(dbContext) { }

        public MobileVersionResponseDTO IsAppOutdated(int version, MobileTypeEnum mobileType, string platform)
        {
            string mobile = mobileType switch
            {
                MobileTypeEnum.Pub => "ALL_PUB",
                MobileTypeEnum.Insp => "ALL_INSP",
                _ => "ALL_PUB",
            };

            bool isAppOutdated = this.Db.NmobileVersions
                .Where(f => f.PageCode == mobile && f.Ostype == platform && f.ForceMinBuildNum > version)
                .Any();

            return new MobileVersionResponseDTO
            {
                IsAppOutdated = isAppOutdated
            };
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new NotImplementedException();
        }
    }
}
