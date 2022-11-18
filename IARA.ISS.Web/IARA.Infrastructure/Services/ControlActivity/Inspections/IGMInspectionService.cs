using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.EntityModels.Entities;
using IARA.Interfaces.CommercialFishing;

namespace IARA.Infrastructure.Services.ControlActivity.Inspections
{
    public class IGMInspectionService : BaseInspectionService<InspectionCheckToolMarkDTO>
    {
        public IGMInspectionService(IARADbContext dbContext, IFishingGearsService fishingGearService)
            : base(dbContext, fishingGearService) { }

        protected override InspectionCheckToolMarkDTO Get(InspectionCheckToolMarkDTO inspection)
        {
            DateTime now = DateTime.Now;

            InspectionCheckToolMarkDTO igmDTO = (
                from check in Db.FishingGearChecks
                join ship in Db.ShipsRegister on check.ShipId equals ship.Id into shipMatchTable
                from shipMatch in shipMatchTable.DefaultIfEmpty()
                join unregisteredShip in Db.UnregisteredVessels on check.UnregisteredShipId equals unregisteredShip.Id into unregisteredShipMatchTable
                from unregisteredShipMatch in unregisteredShipMatchTable.DefaultIfEmpty()
                where check.InspectionId == inspection.Id.Value
                select new InspectionCheckToolMarkDTO
                {
                    InspectedShip = shipMatch == null ? new VesselDuringInspectionDTO
                    {
                        ShipId = null,
                        UnregisteredVesselId = unregisteredShipMatch.Id,
                        IsRegistered = false,
                        Name = unregisteredShipMatch.Name,
                        ExternalMark = unregisteredShipMatch.ExternalMark,
                        CFR = unregisteredShipMatch.Cfr,
                        UVI = unregisteredShipMatch.Uvi,
                        RegularCallsign = unregisteredShipMatch.IrcscallSign,
                        MMSI = unregisteredShipMatch.Mmsi,
                        FlagCountryId = unregisteredShipMatch.FlagCountryId,
                        PatrolVehicleTypeId = unregisteredShipMatch.PatrolVehicleTypeId,
                        VesselTypeId = unregisteredShipMatch.VesselTypeId,
                        IsActive = unregisteredShipMatch.IsActive,
                    } : new VesselDuringInspectionDTO
                    {
                        ShipId = shipMatch.Id,
                        UnregisteredVesselId = null,
                        IsRegistered = true,
                        Name = shipMatch.Name,
                        ExternalMark = shipMatch.ExternalMark,
                        CFR = shipMatch.Cfr,
                        UVI = shipMatch.Uvi,
                        RegularCallsign = shipMatch.IrcscallSign,
                        MMSI = shipMatch.Mmsi,
                        FlagCountryId = shipMatch.FlagCountryId,
                        PatrolVehicleTypeId = null,
                        VesselTypeId = shipMatch.VesselTypeId,
                        IsActive = shipMatch.ValidFrom <= now && shipMatch.ValidTo >= now,
                    },
                    PoundNetId = check.PoundNetId,
                    CheckReasonId = check.CheckReasonId,
                    RecheckReasonId = check.RecheckReasonId,
                    OtherRecheckReason = check.RecheckDescription,
                }).Single();

            List<InspectionPermitDTO> permits = GetPermitLicenses(inspection.Id.Value, SubjectRoleEnum.Inspected);

            if (permits.Count == 1)
            {
                igmDTO.PermitId = permits[0].PermitLicenseId;
            }

            igmDTO.FishingGears = GetFishingGears(inspection.Id.Value);
            igmDTO.Port = GetLastPort(inspection.Id.Value);

            return AssignFromBase(igmDTO, inspection);
        }

        protected override void Submit(InspectionRegister inspDbEntry, InspectionCheckToolMarkDTO igmDTO)
        {
            FishingGearCheck newCmuDbEntry = new()
            {
                Inspection = inspDbEntry,
                PoundNetId = igmDTO.PoundNetId,
                CheckReasonId = igmDTO.CheckReasonId,
                RecheckReasonId = igmDTO.RecheckReasonId,
                RecheckDescription = igmDTO.OtherRecheckReason,
                IsActive = true,
            };

            if (igmDTO.InspectedShip != null)
            {
                if (igmDTO.InspectedShip.ShipId.HasValue && igmDTO.InspectedShip.IsRegistered == true)
                {
                    newCmuDbEntry.Ship = Db.ShipsRegister.First(x => x.Id == igmDTO.InspectedShip.ShipId.Value);
                }
                else
                {
                    newCmuDbEntry.UnregisteredShip = AddUnregisteredShip(igmDTO.InspectedShip);
                }
            }

            if (igmDTO.PermitId != null)
            {
                AddPermitLicenses(inspDbEntry, new List<InspectionPermitDTO>
                {
                    new InspectionPermitDTO
                    {
                        PermitLicenseId = igmDTO.PermitId.Value,
                        CheckValue = InspectionToggleTypesEnum.Y,
                    }
                }, SubjectRoleEnum.Inspected);
            }

            AddFishingGears(inspDbEntry, igmDTO.FishingGears);
            AddPortVisit(inspDbEntry, igmDTO.Port, SubjectRoleEnum.Inspected);

            Db.FishingGearChecks.Add(newCmuDbEntry);
        }
    }
}
