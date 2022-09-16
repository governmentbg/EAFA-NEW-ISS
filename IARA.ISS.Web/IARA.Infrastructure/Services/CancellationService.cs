using System;
using System.Linq;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services
{
    public class CancellationService : Service, ICancellationService
    {
        public CancellationService(IARADbContext db)
            : base(db)
        { }

        public CancellationDetailsDTO GetCancellationDetails(int? id)
        {
            CancellationDetailsDTO result = null;

            if (id.HasValue)
            {
                result = (from canc in Db.CancellationDetails
                          where canc.Id == id.Value
                          select new CancellationDetailsDTO
                          {
                              Id = canc.Id,
                              ReasonId = canc.CancelReasonId,
                              Date = canc.CancelDate,
                              Description = canc.Description,
                              IssueOrderNum = canc.IssueOrderNum,
                              IsActive = canc.IsActive
                          }).First();
            }

            return result;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new NotImplementedException();
        }
    }
}
