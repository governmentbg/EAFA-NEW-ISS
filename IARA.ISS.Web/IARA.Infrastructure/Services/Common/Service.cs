using System;
using System.Linq;
using IARA.Common.Exceptions;
using IARA.DataAccess;
using IARA.DomainModels.Nomenclatures;
using IARA.EntityModels.Interfaces;
using IARA.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure
{
    public abstract class Service : BaseService, IService
    {
        protected Service(IARADbContext dbContext)
            : base(dbContext)
        { }

        public abstract SimpleAuditDTO GetSimpleAudit(int id);

        protected SimpleAuditDTO GetSimpleEntityAuditValues<T>(DbSet<T> dbSet, int id)
         where T : class, IAuditEntity, IIdentityRecord
        {
            try
            {
                T entity = dbSet.Where(x => x.Id == id).First();

                return new SimpleAuditDTO
                {
                    CreatedBy = entity.CreatedBy,
                    CreatedOn = entity.CreatedOn,
                    UpdatedBy = entity.UpdatedBy,
                    UpdatedOn = entity.UpdatedOn
                };
            }
            catch (InvalidOperationException ex)
            {
                throw new RecordNotFoundException("Record not found", ex);
            }
        }
    }
}
