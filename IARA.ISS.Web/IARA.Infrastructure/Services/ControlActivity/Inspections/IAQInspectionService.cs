using System.Linq;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.EntityModels.Entities;
using IARA.Interfaces.CommercialFishing;
using NetTopologySuite.Geometries;

namespace IARA.Infrastructure.Services.ControlActivity.Inspections
{
    public class IAQInspectionService : BaseInspectionService<InspectionAquacultureDTO>
    {
        public IAQInspectionService(IARADbContext dbContext, IFishingGearsService fishingGearService)
            : base(dbContext, fishingGearService) { }

        protected override InspectionAquacultureDTO Get(InspectionAquacultureDTO inspection)
        {
            InspectionAquacultureDTO iaqDTO = (
                from insp in Db.AquacultureInspections
                where insp.InspectionId == inspection.Id.Value
                select new InspectionAquacultureDTO
                {
                    AquacultureId = insp.AquacultureRegisterId,
                    RepresentativeComment = insp.RepresentativeComment,
                    OtherFishingGear = insp.OtherFishingToolsDescription,
                    Location = insp.Coordinates != null
                       ? new LocationDTO { Longitude = insp.Coordinates.X, Latitude = insp.Coordinates.Y }
                       : null,
                }).Single();

            iaqDTO.CatchMeasures = GetCatchMeasures(inspection.Id.Value);

            return AssignFromBase(iaqDTO, inspection);
        }

        protected override void Submit(InspectionRegister inspDbEntry, InspectionAquacultureDTO iaqDTO)
        {
            AquacultureInspection newIaqDbEntry = new()
            {
                Inspection = inspDbEntry,
                AquacultureRegisterId = iaqDTO.AquacultureId,
                OtherFishingToolsDescription = iaqDTO.OtherFishingGear,
                RepresentativeComment = iaqDTO.RepresentativeComment,
                Coordinates = iaqDTO.Location != null
                    ? new Point(iaqDTO.Location.Longitude.Value, iaqDTO.Location.Latitude.Value)
                    : null,
                IsActive = true
            };

            AddCatchMeasures(inspDbEntry, iaqDTO.CatchMeasures);

            Db.AquacultureInspections.Add(newIaqDbEntry);
        }
    }
}
