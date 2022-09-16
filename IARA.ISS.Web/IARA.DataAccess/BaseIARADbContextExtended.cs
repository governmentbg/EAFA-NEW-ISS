using System.Collections.Generic;
using IARA.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace IARA.DataAccess
{
    public partial class BaseIARADbContext : ILoggingDbContext, IUsersDbContext
    {
        public bool ApplyComplexAudit { get; set; } = true;

        public Dictionary<string, string> GetTableAndEntityNames()
        {
            return DataAccessUtils.GetTableAndEntityNames(this);
        }

        public void NoTracking()
        {
            base.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public void StartTracking()
        {
            base.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
        }
    }
}
