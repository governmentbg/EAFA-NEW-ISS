using System;
using System.Collections.Generic;
using IARA.DataAccess;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces.Nomenclatures;

namespace IARA.Infrastructure.Services.SystemLog
{
    public class SystemLogNomenclaturesService : Service, ISystemLogNomenclaturesService
    {
        public SystemLogNomenclaturesService(IARADbContext dbContext) : base(dbContext)
        {

        }

        public List<NomenclatureDTO> GetActionTypeCategories()
        {
            return GetCodeNomenclature(Db.NauditLogActionTypes);
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new NotImplementedException();
        }
    }
}
