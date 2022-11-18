using System;
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
    public class IBSInspectionService : BaseInspectionService<InspectionAtSeaDTO>
    {
        public IBSInspectionService(IARADbContext dbContext, IFishingGearsService fishingGearService)
            : base(dbContext, fishingGearService) { }

        protected override InspectionAtSeaDTO Get(InspectionAtSeaDTO inspection)
        {
            DateTime now = DateTime.Now;

            InspectionAtSeaDTO ibsDTO = (
                from insp in Db.ShipInspections
                where insp.InspectionId == inspection.Id.Value
                    && insp.InspectedShipType == nameof(SubjectRoleEnum.Inspected)
                select new InspectionAtSeaDTO
                {
                    CaptainComment = insp.CaptainComment,
                    InspectedShip = new VesselDuringInspectionDTO
                    {
                        ShipId = insp.InspectiedShipId,
                        UnregisteredVesselId = insp.InspectedUnregisteredShipId,
                        Location = insp.InspectedShipCoordinates == null ? null : new LocationDTO
                        {
                            Longitude = insp.InspectedShipCoordinates.X,
                            Latitude = insp.InspectedShipCoordinates.Y
                        },
                        LocationDescription = insp.InspectedShipLocation,
                        CatchZoneId = insp.InspectedShipCatchZoneId,
                    },
                }).SingleOrDefault();

            ibsDTO.InspectedShip = GetInspectedShip(ibsDTO.InspectedShip);
            ibsDTO.FishingGears = GetFishingGears(inspection.Id.Value);
            ibsDTO.CatchMeasures = GetCatchMeasures(inspection.Id.Value, SubjectRoleEnum.Inspected);
            ibsDTO.Personnel = GetPersonnel(inspection.Id.Value, SubjectRoleEnum.Inspected);
            ibsDTO.LastPortVisit = GetLastPort(inspection.Id.Value, SubjectRoleEnum.Inspected);
            ibsDTO.PermitLicenses = GetPermitLicenses(inspection.Id.Value, SubjectRoleEnum.Inspected);
            ibsDTO.Permits = GetPermits(inspection.Id.Value, SubjectRoleEnum.Inspected);
            ibsDTO.LogBooks = GetLogBooks(inspection.Id.Value, SubjectRoleEnum.Inspected);

            return AssignFromBase(ibsDTO, inspection);
        }

        protected override void Submit(InspectionRegister inspDbEntry, InspectionAtSeaDTO ibsDTO)
        {
            ShipInspection newIbsDbEntry = new()
            {
                Inspection = inspDbEntry,
                CaptainComment = ibsDTO.CaptainComment,
                InspectedShipCoordinates = ibsDTO.InspectedShip?.Location != null
                    ? new Point(ibsDTO.InspectedShip.Location.Longitude.Value, ibsDTO.InspectedShip.Location.Latitude.Value)
                    : null,
                InspectedShipType = nameof(SubjectRoleEnum.Inspected),
                InspectedShipCatchZoneId = ibsDTO.InspectedShip?.CatchZoneId,
                InspectedShipLocation = ibsDTO.InspectedShip?.LocationDescription,
                IsActive = true,
            };

            if (ibsDTO.InspectedShip != null)
            {
                if (ibsDTO.InspectedShip.ShipId.HasValue && ibsDTO.InspectedShip.IsRegistered == true)
                {
                    newIbsDbEntry.InspectiedShip = Db.ShipsRegister.First(x => x.Id == ibsDTO.InspectedShip.ShipId.Value);
                }
                else
                {
                    newIbsDbEntry.InspectedUnregisteredShip = AddUnregisteredShip(ibsDTO.InspectedShip);
                }
            }

            AddCatchMeasures(inspDbEntry, ibsDTO.CatchMeasures, SubjectRoleEnum.Inspected);
            AddFishingGears(inspDbEntry, ibsDTO.FishingGears);
            AddPortVisit(inspDbEntry, ibsDTO.LastPortVisit, SubjectRoleEnum.Inspected);

            AddPermitLicenses(inspDbEntry, ibsDTO.PermitLicenses, SubjectRoleEnum.Inspected);
            AddPermits(inspDbEntry, ibsDTO.Permits, SubjectRoleEnum.Inspected);
            AddLogBooks(inspDbEntry, ibsDTO.LogBooks, SubjectRoleEnum.Inspected);

            Db.ShipInspections.Add(newIbsDbEntry);
        }
    }
}
