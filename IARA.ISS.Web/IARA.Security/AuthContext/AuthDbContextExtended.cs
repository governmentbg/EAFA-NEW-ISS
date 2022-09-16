using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using IARA.DataAccess.Abstractions;
using IARA.EntityModels.Entities;
using IARA.EntityModels.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IARA.Security.AuthContext
{
    public partial class AuthDbContext : ILoggingDbContext, IUsersDbContext
    {
        public Dictionary<string, string> GetTableAndEntityNames()
        {
            return DataAccessUtils.GetTableAndEntityNames(this);
        }

        public override int SaveChanges()
        {
            DataAccessUtils.ApplyAudit(this.ChangeTracker);
            return base.SaveChanges();
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            //modelBuilder.Ignore<ScientificPermitReason>();
            //modelBuilder.Ignore<AquacultureFacilityCoordinate>();
            //modelBuilder.Ignore<FishermanInspection>();
            //modelBuilder.Ignore<FishingCatchRecord>();
            //modelBuilder.Ignore<InspectionPatrolVehicle>();
            //modelBuilder.Ignore<NcatchZone>();
            //modelBuilder.Ignore<ObservationAtSea>();
            //modelBuilder.Ignore<PoundNetCoordinate>();
            //modelBuilder.Ignore<ShipInspection>();
            //modelBuilder.Ignore<TransportVehicleInspection>();
            //modelBuilder.Ignore<WaterObjectCheck>();

            List<Type> authDbContextTypes = typeof(AuthDbContext).GetProperties()
                       .Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                       .Select(x => x.PropertyType.GetGenericArguments()[0])
                       .ToList();

            MethodInfo ignoreMethod = typeof(ModelBuilder).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.Name == nameof(ModelBuilder.Ignore) && x.IsGenericMethod)
                .First();

            foreach (var entityType in typeof(IAuditEntity).Assembly.GetTypes().Where(x => !x.IsInterface && x.GetCustomAttributes(typeof(TableAttribute), true).Any()))
            {
                if (!authDbContextTypes.Contains(entityType))
                {
                    ignoreMethod.MakeGenericMethod(entityType).Invoke(modelBuilder, new object[] { });
                }
            }
        }
    }
}
