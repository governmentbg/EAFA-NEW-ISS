using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Common;
using IARA.DataAccess.Abstractions;
using IARA.Logging.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace IARA.DataAccess
{
    public partial class IARADbContext : IDisposable
    {
        private BaseIARADbContext dbContext;

        private IUserActionsAuditLogger userActionsAuditLogger;
        private ScopedServiceProviderFactory scopedServiceProviderFactory;

        public IARADbContext(BaseIARADbContext dbContext, IUserActionsAuditLogger userActionsAuditLogger, ScopedServiceProviderFactory scopedServiceProviderFactory)
        {
            this.dbContext = dbContext;
            this.userActionsAuditLogger = userActionsAuditLogger;
            this.scopedServiceProviderFactory = scopedServiceProviderFactory;
        }

        public DatabaseFacade Database
        {
            get
            {
                return dbContext.Database;
            }
        }

        public IModel Model
        {
            get
            {
                return dbContext.Model;
            }
        }

        public void Dispose()
        {
            try
            {
                dbContext.Dispose();
            }
            catch (InvalidOperationException)
            {

            }
        }

        public EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
            where TEntity : class
        {
            return dbContext.Entry(entity);
        }

        public DbSet<TEntity> Set<TEntity>()
            where TEntity : class
        {
            return dbContext.Set<TEntity>();
        }

        public IDbContextTransaction BeginTransaction()
        {
            this.dbContext.Database.AutoTransactionsEnabled = false;
            return this.dbContext.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            this.dbContext.Database.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            this.dbContext.Database.RollbackTransaction();
        }

        public Dictionary<string, string> GetTableAndEntityNames()
        {
            return DataAccessUtils.GetTableAndEntityNames(this.dbContext);
        }

        public int SaveChanges()
        {
            List<ChangedEntityModel> entities = DataAccessUtils.ApplyAudit(dbContext.ChangeTracker);
            int result = dbContext.SaveChanges();

            if (ApplyComplexAudit)
            {
                userActionsAuditLogger.ApplyComplexAudit(scopedServiceProviderFactory, entities);
            }

            return result;
        }

        public async Task<int> SaveChangesAsync()
        {
            List<ChangedEntityModel> entities = DataAccessUtils.ApplyAudit(dbContext.ChangeTracker);
            int result = await dbContext.SaveChangesAsync();

            if (ApplyComplexAudit)
            {
                userActionsAuditLogger.ApplyComplexAudit(scopedServiceProviderFactory, entities);
            }

            return result;
        }

        public void NoTracking()
        {
            dbContext.NoTracking();
        }

        public bool ApplyComplexAudit
        {
            get
            {
                return dbContext.ApplyComplexAudit;
            }
            set
            {
                dbContext.ApplyComplexAudit = value;
            }
        }

        public void StartTracking()
        {
            dbContext.StartTracking();
        }
    }
}
