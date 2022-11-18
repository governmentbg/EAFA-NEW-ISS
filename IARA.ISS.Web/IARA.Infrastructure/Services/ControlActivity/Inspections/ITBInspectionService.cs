using System;
using System.Collections.Generic;
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
    public class ITBInspectionService : BaseInspectionService<InspectionTransboardingDTO>
    {
        public ITBInspectionService(IARADbContext dbContext, IFishingGearsService fishingGearService)
            : base(dbContext, fishingGearService) { }

        protected override InspectionTransboardingDTO Get(InspectionTransboardingDTO inspection)
        {
            InspectionTransboardingShipDTO receivingShipInspection = GetRegisterITBShip(inspection.Id.Value, SubjectRoleEnum.TransboardReceiver);

            InspectionTransboardingShipDTO sendingShipInspection = GetRegisterITBShip(inspection.Id.Value, SubjectRoleEnum.TransboardSender);

            List<InspectedFishingGearDTO> fishingGears = GetFishingGears(inspection.Id.Value);

            InspectionTransboardingDTO itbDTO = new()
            {
                TransboardedCatchMeasures = GetCatchMeasures(inspection.Id.Value, SubjectRoleEnum.Inspected),
                SendingShipInspection = sendingShipInspection,
                ReceivingShipInspection = receivingShipInspection,
                FishingGears = fishingGears,
            };

            return AssignFromBase(itbDTO, inspection);
        }

        protected override void Submit(InspectionRegister inspDbEntry, InspectionTransboardingDTO itbDTO)
        {
            AddCatchMeasures(inspDbEntry, itbDTO.TransboardedCatchMeasures, SubjectRoleEnum.Inspected);
            AddFishingGears(inspDbEntry, itbDTO.FishingGears);

            if (itbDTO.ReceivingShipInspection != null)
            {
                AddRegisterITBShip(itbDTO.ReceivingShipInspection, inspDbEntry, SubjectRoleEnum.TransboardReceiver);
            }

            if (itbDTO.SendingShipInspection != null)
            {
                AddRegisterITBShip(itbDTO.SendingShipInspection, inspDbEntry, SubjectRoleEnum.TransboardSender);
            }
        }

        private InspectionTransboardingShipDTO GetRegisterITBShip(int inspectionId, SubjectRoleEnum role)
        {
            DateTime now = DateTime.Now;

            InspectionTransboardingShipDTO ibsDTO = (
                from insp in Db.ShipInspections
                where insp.InspectionId == inspectionId
                    && insp.InspectedShipType == role.ToString()
                select new InspectionTransboardingShipDTO
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

            if (ibsDTO != null)
            {
                ibsDTO.InspectedShip = GetInspectedShip(ibsDTO.InspectedShip);
                ibsDTO.CatchMeasures = GetCatchMeasures(inspectionId, role);
                ibsDTO.Personnel = GetPersonnel(inspectionId, role);
                ibsDTO.Checks = GetChecks(inspectionId, role);
                ibsDTO.LastPortVisit = GetLastPort(inspectionId, role);
                ibsDTO.PermitLicenses = GetPermitLicenses(inspectionId, role);
                ibsDTO.Permits = GetPermits(inspectionId, role);
                ibsDTO.LogBooks = GetLogBooks(inspectionId, role);
            }

            return ibsDTO;
        }

        private void AddRegisterITBShip(InspectionTransboardingShipDTO dto, InspectionRegister newInspDbEntry, SubjectRoleEnum shipType)
        {
            ShipInspection sendingIbsDbEntry = new()
            {
                Inspection = newInspDbEntry,
                CaptainComment = dto.CaptainComment,
                InspectedShipCoordinates = dto.InspectedShip?.Location != null
                    ? new Point(dto.InspectedShip.Location.Longitude.Value, dto.InspectedShip.Location.Latitude.Value)
                    : null,
                InspectedShipType = shipType.ToString(),
                InspectedShipCatchZoneId = dto.InspectedShip?.CatchZoneId,
                InspectedShipLocation = dto.InspectedShip?.LocationDescription,
                IsActive = true,
            };

            if (dto.InspectedShip != null)
            {
                if (dto.InspectedShip.ShipId.HasValue && dto.InspectedShip.IsRegistered == true)
                {
                    sendingIbsDbEntry.InspectiedShip = Db.ShipsRegister.First(x => x.Id == dto.InspectedShip.ShipId.Value);
                }
                else
                {
                    sendingIbsDbEntry.InspectedUnregisteredShip = AddUnregisteredShip(dto.InspectedShip);
                }
            }

            AddPortVisit(newInspDbEntry, dto.LastPortVisit, shipType);
            AddCatchMeasures(newInspDbEntry, dto.CatchMeasures, shipType);
            AddInspectionChecks(newInspDbEntry, dto.Checks, shipType);

            AddPermitLicenses(newInspDbEntry, dto.PermitLicenses, shipType);
            AddPermits(newInspDbEntry, dto.Permits, shipType);
            AddLogBooks(newInspDbEntry, dto.LogBooks, shipType);

            if (dto.Personnel != null)
            {
                AddInspectedPersons(newInspDbEntry, dto.Personnel, shipType);
            }

            Db.ShipInspections.Add(sendingIbsDbEntry);
        }
    }
}
