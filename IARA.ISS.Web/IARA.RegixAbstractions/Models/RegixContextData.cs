using IARA.Common;
using IARA.DataAccess;

namespace IARA.RegixAbstractions.Models
{
    public class RegixContextData<T, TCompare> : BaseContextData
    {
        public RegixContextData(CheckContext context)
        {
            this.ApplicationHistoryId = context.ApplicationHistoryId;
            this.ApplicationId = context.ApplicationId;
            this.ServiceURI = context.ServiceURI;
            this.ServiceType = context.ServiceType;
            this.EGN = context.EGN;
            this.EmployeeIdentifier = context.EmployeeIdentifier;
            this.EmployeeNames = context.EmployeeNames;
        }

        public T Context { get; set; }
        public TCompare CompareWithObject { get; set; }
        public IARADbContext Db { get; set; }
        public IScopedServiceProvider ServiceProvider { get; set; }
    }
}
