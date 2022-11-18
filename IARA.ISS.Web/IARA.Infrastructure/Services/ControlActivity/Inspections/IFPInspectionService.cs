using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.EntityModels.Entities;
using IARA.Interfaces.CommercialFishing;
using NetTopologySuite.Geometries;

namespace IARA.Infrastructure.Services.ControlActivity.Inspections
{
    public class IFPInspectionService : BaseInspectionService<InspectionFisherDTO>
    {
        public IFPInspectionService(IARADbContext dbContext, IFishingGearsService fishingGearService)
            : base(dbContext, fishingGearService) { }

        protected override InspectionFisherDTO Get(InspectionFisherDTO inspection)
        {
            InspectionFisherDTO ifpDTO = (
                from insp in Db.FishermanInspections
                join unregisteredPerson in Db.UnregisteredPersons on insp.UnregisteredPersonId equals unregisteredPerson.Id into unregisteredPersonMatchTable
                from unregisteredPersonMatch in unregisteredPersonMatchTable.DefaultIfEmpty()
                where insp.InspectionId == inspection.Id.Value
                select new InspectionFisherDTO
                {
                    TicketNum = insp.TicketNum,
                    FishingRodsCount = insp.FishingRodCount,
                    FishingHooksCount = insp.FishingHooksCount,
                    FishermanComment = insp.FishermanComment,
                    InspectionAddress = insp.InspectionLocation,
                    InspectionLocation = insp.InpectionLocationCoordinates != null
                       ? new LocationDTO { Longitude = insp.InpectionLocationCoordinates.X, Latitude = insp.InpectionLocationCoordinates.Y }
                       : null,
                }).Single();

            ifpDTO.CatchMeasures = GetCatchMeasures(inspection.Id.Value);

            return AssignFromBase(ifpDTO, inspection);
        }

        protected override void Submit(InspectionRegister inspDbEntry, InspectionFisherDTO ifpDTO)
        {
            FishermanInspection newIfpDbEntry = new()
            {
                Inspection = inspDbEntry,
                TicketNum = ifpDTO.TicketNum,
                FishingRodCount = ifpDTO.FishingRodsCount,
                FishingHooksCount = ifpDTO.FishingHooksCount,
                FishermanComment = ifpDTO.FishermanComment,
                InspectionLocation = ifpDTO.InspectionAddress,
                InpectionLocationCoordinates = ifpDTO.InspectionLocation != null
                    ? new Point(ifpDTO.InspectionLocation.Longitude.Value, ifpDTO.InspectionLocation.Latitude.Value)
                    : null,
                IsActive = true
            };

            InspectionSubjectPersonnelDTO fisherDTO = ifpDTO.Personnel
                .SingleOrDefault(f => f.Type == InspectedPersonTypeEnum.CaptFshmn);

            if (fisherDTO != null)
            {
                newIfpDbEntry.UnregisteredPerson = AddUnregisteredPerson(fisherDTO);
            }

            AddCatchMeasures(inspDbEntry, ifpDTO.CatchMeasures);

            Db.FishermanInspections.Add(newIfpDbEntry);
        }
    }
}
