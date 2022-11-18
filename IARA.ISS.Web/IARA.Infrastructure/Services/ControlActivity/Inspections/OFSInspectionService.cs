using System;
using System.Collections.Generic;
using System.Linq;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.EntityModels.Entities;
using IARA.Interfaces.CommercialFishing;
using NetTopologySuite.Geometries;

namespace IARA.Infrastructure.Services.ControlActivity.Inspections
{
    public class OFSInspectionService : BaseInspectionService<InspectionObservationAtSeaDTO>
    {
        public OFSInspectionService(IARADbContext dbContext, IFishingGearsService fishingGearService)
            : base(dbContext, fishingGearService) { }

        protected override InspectionObservationAtSeaDTO Get(InspectionObservationAtSeaDTO inspection)
        {
            DateTime now = DateTime.Now;

            InspectionObservationAtSeaDTO ofsDTO = (
                from observation in Db.ObservationsAtSea
                where observation.InspectionId == inspection.Id.Value
                select new InspectionObservationAtSeaDTO
                {
                    ObservedVessel = new VesselDuringInspectionDTO
                    {
                        ShipId = observation.ObservedShipId,
                        UnregisteredVesselId = observation.ObservedUnregisteredShipId,
                        Location = observation.ObservedShipCoordinates == null ? null : new LocationDTO
                        {
                            Latitude = observation.ObservedShipCoordinates.Y,
                            Longitude = observation.ObservedShipCoordinates.X
                        },
                        LocationDescription = observation.ObservedShipLocation,
                        CatchZoneId = observation.ObservedShipCatchZoneId,
                    },
                    Course = observation.Direction,
                    Speed = observation.Speed,
                    HasShipContact = observation.HasShipContact,
                    HasShipCommunication = observation.HasShipCommunication,
                    ShipCommunicationDescription = observation.ShipCommunicationDescr,
                }).Single();

            ofsDTO.ObservedVessel = GetInspectedShip(ofsDTO.ObservedVessel);

            ofsDTO.ObservedVesselActivities = GetVesselActivities(inspection.Id.Value);

            ofsDTO.ObservationTools = GetObservationTools(inspection.Id.Value);

            return AssignFromBase(ofsDTO, inspection);
        }

        protected override void Submit(InspectionRegister inspDbEntry, InspectionObservationAtSeaDTO ofsDTO)
        {
            ObservationAtSea newObsDbEntry = new()
            {
                Inspection = inspDbEntry,
                ObservedShipCoordinates = ofsDTO.ObservedVessel?.Location != null
                    ? new Point(ofsDTO.ObservedVessel.Location.Longitude.Value, ofsDTO.ObservedVessel.Location.Latitude.Value)
                    : null,
                Direction = ofsDTO.Course,
                Speed = ofsDTO.Speed,
                HasShipContact = ofsDTO.HasShipContact,
                HasShipCommunication = ofsDTO.HasShipCommunication,
                ShipCommunicationDescr = ofsDTO.ShipCommunicationDescription,
                ObservedShipCatchZoneId = ofsDTO.ObservedVessel?.CatchZoneId,
                ObservedShipLocation = ofsDTO.ObservedVessel?.LocationDescription,
                IsActive = true
            };

            if (ofsDTO.ObservedVessel != null)
            {
                if (ofsDTO.ObservedVessel.ShipId.HasValue && ofsDTO.ObservedVessel.IsRegistered == true)
                {
                    newObsDbEntry.ObservedShip = Db.ShipsRegister.First(x => x.Id == ofsDTO.ObservedVessel.ShipId.Value);
                }
                else
                {
                    newObsDbEntry.ObservedUnregisteredShip = AddUnregisteredShip(ofsDTO.ObservedVessel);
                }
            }

            AddObservationTools(inspDbEntry, ofsDTO.ObservationTools);
            AddInspectionVesselActivities(inspDbEntry, ofsDTO.ObservedVesselActivities);

            Db.ObservationsAtSea.Add(newObsDbEntry);
        }

        private List<InspectionVesselActivityNomenclatureDTO> GetVesselActivities(int inspectionId)
        {
            List<InspectionVesselActivityNomenclatureDTO> result = (
                from iva in Db.InspectionVesselActivities
                join va in Db.NvesselActivities on iva.VesselActivityId equals va.Id
                where iva.InspectionId == inspectionId
                    && iva.IsActive
                select new InspectionVesselActivityNomenclatureDTO
                {
                    Value = va.Id,
                    Code = va.Code,
                    DisplayName = va.Name,
                    Description = iva.ActivityDescr,
                    IsFishingActivity = va.IsFishingActivity,
                    HasAdditionalDescr = va.HasAdditionalDescr,
                    IsActive = iva.IsActive,
                }
            ).ToList();

            return result;
        }

        private List<InspectionObservationToolDTO> GetObservationTools(int inspectionId)
        {
            List<InspectionObservationToolDTO> result = (
                from inspectionObsTool in Db.InspectionObservationTools
                where inspectionObsTool.InspectionId == inspectionId
                select new InspectionObservationToolDTO
                {
                    ObservationToolId = inspectionObsTool.ObservationToolId,
                    Description = inspectionObsTool.ObservationToolDesc,
                    IsOnBoard = inspectionObsTool.IsOnBoard,
                    IsActive = inspectionObsTool.IsActive
                }
            ).ToList();

            return result;
        }
    }
}
