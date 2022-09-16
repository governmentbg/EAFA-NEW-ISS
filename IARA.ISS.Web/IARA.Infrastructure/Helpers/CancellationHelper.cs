using System.Linq;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.EntityModels.Entities;
using IARA.EntityModels.Interfaces;

namespace IARA.Infrastructure.Helpers
{
    internal static class CancellationHelper
    {
        public static CancellationDetail AddOrEditCancellationDetails<TEntity>(this IARADbContext db, TEntity entry, CancellationDetailsDTO details)
            where TEntity : class, ICancellableEntity
        {
            CancellationDetail cancellation = null;

            if (entry.CancellationDetailsId.HasValue)
            {
                cancellation = (from cd in db.CancellationDetails
                                where cd.Id == entry.CancellationDetailsId.Value
                                select cd).Single();
            }

            if (details != null)
            {
                if (cancellation != null)
                {
                    cancellation.CancelReasonId = details.ReasonId;
                    cancellation.IssueOrderNum = details.IssueOrderNum;
                    cancellation.CancelDate = details.Date;
                    cancellation.Description = details.Description;
                    cancellation.IsActive = details.IsActive;
                }
                else
                {
                    entry.CancellationDetails = new CancellationDetail
                    {
                        CancelReasonId = details.ReasonId,
                        IssueOrderNum = details.IssueOrderNum,
                        CancelDate = details.Date,
                        Description = details.Description,
                        IsActive = true
                    };
                }
            }
            else
            {
                if (cancellation != null)
                {
                    cancellation.IsActive = false;
                }
            }

            return cancellation;
        }
    }
}
