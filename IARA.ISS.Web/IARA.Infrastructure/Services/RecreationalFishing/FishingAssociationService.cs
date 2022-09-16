using System.Collections.Generic;
using System.Linq;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.FishingAssociations;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services
{
    public class FishingAssociationService : IFishingAssociationService
    {
        private readonly IARADbContext Db;

        public FishingAssociationService(IARADbContext db)
        {
            Db = db;
        }

        public IEnumerable<FishingAssociationDTO> GetAllApprovedFishingAssociations()
        {
            return (from fa in this.Db.FishingAssociations
                    join legel in Db.Legals on fa.AssociationLegalId equals legel.Id
                    where !fa.IsCanceled
                    select new FishingAssociationDTO
                    {
                        Id = fa.Id,
                        Name = legel.Name,
                        IsActive = fa.IsActive
                    }).ToList();
        }
    }
}
