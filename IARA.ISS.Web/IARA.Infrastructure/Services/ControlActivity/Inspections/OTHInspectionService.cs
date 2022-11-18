using System.Linq;
using System.ServiceModel;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.EntityModels.Entities;
using IARA.Interfaces.CommercialFishing;

namespace IARA.Infrastructure.Services.ControlActivity.Inspections
{
    public class OTHInspectionService : BaseInspectionService<InspectionConstativeProtocolDTO>
    {
        public OTHInspectionService(IARADbContext dbContext, IFishingGearsService fishingGearService)
            : base(dbContext, fishingGearService) { }

        protected override InspectionConstativeProtocolDTO Get(InspectionConstativeProtocolDTO inspection)
        {
            InspectionConstativeProtocolDTO othDTO = (
                from cpi in Db.ConstativeProtocolInspections
                where cpi.InspectionId == inspection.Id
                select new InspectionConstativeProtocolDTO
                {
                    InspectedObjectName = cpi.InspectedObjectName,
                    InspectedPersonName = cpi.InspectedPersonName,
                    InspectorName = cpi.InspectorName,
                    Location = cpi.Location,
                    Witness1Name = cpi.Witness1Name,
                    Witness2Name = cpi.Witness2Name,
                }).Single();

            othDTO.FishingGears = (
                from ifg in Db.InspectedFishingGears
                join fishingGear in Db.FishingGearRegisters on ifg.InspectedFishingGearId equals fishingGear.Id
                where ifg.InspectionId == inspection.Id
                select new InspectedCPFishingGearDTO
                {
                    Id = fishingGear.Id,
                    Description = fishingGear.Description,
                    FishingGearId = fishingGear.FishingGearTypeId,
                    GearCount = fishingGear.GearCount,
                    Length = fishingGear.Length.Value,
                    IsStored = ifg.IsStored.Value,
                    IsTaken = ifg.IsTaken.Value,
                }
            ).ToList();

            othDTO.Catches = (
                from icm in Db.InspectionCatchMeasures
                where icm.InspectionId == inspection.Id
                select new InspectedCPCatchDTO
                {
                    Id = icm.Id,
                    FishId = icm.FishId.Value,
                    CatchQuantity = icm.CatchQuantity.Value,
                    IsDestroyed = icm.IsDestroyed.Value,
                    IsTaken = icm.IsTaken.Value,
                    IsDonated = icm.IsDonated.Value,
                    IsStored = icm.IsStored.Value,
                }
            ).ToList();

            return AssignFromBase(othDTO, inspection);
        }

        protected override void Submit(InspectionRegister inspDbEntry, InspectionConstativeProtocolDTO item)
        {
            throw new ActionNotSupportedException();
        }
    }
}
