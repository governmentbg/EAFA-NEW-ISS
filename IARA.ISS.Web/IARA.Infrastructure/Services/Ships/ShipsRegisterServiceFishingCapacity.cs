using System.Linq;
using System.Transactions;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.FishingCapacity;
using IARA.DomainModels.DTOModels.FishingCapacity.IncreaseCapacity;
using IARA.DomainModels.DTOModels.FishingCapacity.ReduceCapacity;
using IARA.DomainModels.DTOModels.ShipsRegister;
using IARA.DomainModels.DTOModels.ShipsRegister.IncreaseCapacity;
using IARA.DomainModels.DTOModels.ShipsRegister.ReduceCapacity;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services
{
    public partial class ShipsRegisterService : Service, IShipsRegisterService
    {
        public ShipRegisterEditDTO GetShipFromIncreaseCapacityApplication(int applicationId)
        {
            CapacityChangeHistoryDTO change = fishingCapacityService.GetCapacityChangeHistory(applicationId, RecordTypesEnum.Application);

            int shipId = (from cap in Db.ShipCapacityRegister
                          where cap.Id == change.ShipCapacityId.Value
                          select cap.ShipId).First();

            return GetShip(shipId);
        }

        public ShipRegisterEditDTO GetShipFromReduceCapacityApplication(int applicationId)
        {
            CapacityChangeHistoryDTO change = fishingCapacityService.GetCapacityChangeHistory(applicationId, RecordTypesEnum.Application);

            int shipId = (from cap in Db.ShipCapacityRegister
                          where cap.Id == change.ShipCapacityId.Value
                          select cap.ShipId).First();

            return GetShip(shipId);
        }

        public void CompleteShipIncreaseCapacityApplication(ShipRegisterIncreaseCapacityDTO ships)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (ShipRegisterEditDTO ship in ships.Ships)
                {
                    EditShip(ship, ships.ApplicationId.Value, ship.ShipUID);
                }

                IncreaseFishingCapacityApplicationDTO application = fishingCapacityService.GetIncreaseFishingCapacityApplication(ships.ApplicationId.Value);
                fishingCapacityService.CompleteIncreaseFishingCapacityApplication(application);

                scope.Complete();
            }

            stateMachine.Act(ships.ApplicationId.Value);
        }

        public void CompleteShipReduceCapacityApplication(ShipRegisterReduceCapacityDTO ships)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (ShipRegisterEditDTO ship in ships.Ships)
                {
                    EditShip(ship, ships.ApplicationId.Value, ship.ShipUID);
                }

                ReduceFishingCapacityApplicationDTO application = fishingCapacityService.GetReduceFishingCapacityApplication(ships.ApplicationId.Value);
                fishingCapacityService.CompleteReduceFishingCapacityApplication(application);

                scope.Complete();
            }

            stateMachine.Act(ships.ApplicationId.Value);
        }
    }
}
