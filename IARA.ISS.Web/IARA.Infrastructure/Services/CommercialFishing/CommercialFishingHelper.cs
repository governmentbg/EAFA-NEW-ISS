using System.Linq;
using IARA.DataAccess;

namespace IARA.Infrastructure.Services.CommercialFishing
{
    internal static class CommercialFishingHelper
    {
        public static List<int> GetValidShipPermitIds(IARADbContext db, int shipId)
        {
            int shipUId = (from ship in db.ShipsRegister
                           where ship.Id == shipId
                           select ship.ShipUid).First();

            List<int> result = (from permit in db.CommercialFishingPermitRegisters
                                join ship in db.ShipsRegister on permit.ShipId equals ship.Id
                                where permit.RecordType == nameof(RecordTypesEnum.Register)
                                      && ship.ShipUid == shipUId
                                      && !permit.IsSuspended
                                      && permit.IsActive
                                select permit.Id).ToList();
            return result;
        }
    }
}
