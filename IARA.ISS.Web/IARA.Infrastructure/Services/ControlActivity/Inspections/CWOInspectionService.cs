using System.Collections.Generic;
using System.Linq;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.EntityModels.Entities;
using IARA.Interfaces.CommercialFishing;
using NetTopologySuite.Geometries;

namespace IARA.Infrastructure.Services.ControlActivity.Inspections
{
    public class CWOInspectionService : BaseInspectionService<InspectionCheckWaterObjectDTO>
    {
        public CWOInspectionService(IARADbContext dbContext, IFishingGearsService fishingGearService)
            : base(dbContext, fishingGearService) { }

        protected override InspectionCheckWaterObjectDTO Get(InspectionCheckWaterObjectDTO inspection)
        {
            InspectionCheckWaterObjectDTO cwoDTO = (
                from check in Db.WaterObjectChecks
                where check.InspectionId == inspection.Id.Value
                select new InspectionCheckWaterObjectDTO
                {
                    ObjectName = check.Name,
                    WaterObjectTypeId = check.WaterObjectTypeId,
                    WaterObjectLocation = new LocationDTO { Longitude = check.WaterObjectCoordinates.X, Latitude = check.WaterObjectCoordinates.Y },
                }).Single();

            cwoDTO.FishingGears = (
                from ifg in Db.InspectedFishingGears
                join ifgn in Db.FishingGearRegisters on ifg.InspectedFishingGearId equals ifgn.Id
                where ifg.InspectionId == inspection.Id.Value
                    && ifg.IsActive
                select new WaterInspectionFishingGearDTO
                {
                    Id = ifgn.Id,
                    TypeId = ifgn.FishingGearTypeId,
                    Count = ifgn.GearCount,
                    Length = ifgn.Length,
                    Height = ifgn.Height,
                    NetEyeSize = ifgn.NetEyeSize,
                    HookCount = ifgn.HookCount,
                    Description = ifgn.Description,
                    TowelLength = ifgn.TowelLength,
                    HouseLength = ifgn.HouseLength,
                    HouseWidth = ifgn.HouseWidth,
                    HasPingers = ifgn.HasPinger,
                    StorageLocation = ifg.StorageLocation,
                    IsTaken = ifg.IsTaken ?? false,
                    IsStored = ifg.IsStored ?? false,
                    IsActive = ifgn.IsActive,
                }
            ).ToList();

            FishingGearService.MapFishingGearMarksAndPingers(cwoDTO.FishingGears.ConvertAll(f => f as FishingGearDTO));

            foreach (WaterInspectionFishingGearDTO fishingGear in cwoDTO.FishingGears)
            {
                fishingGear.MarksNumbers = string.Join(", ", fishingGear.Marks.Where(x => x.IsActive).Select(x => x.Number));
            }

            cwoDTO.Vessels = (
                from vessel in Db.InspectionVessels
                where vessel.InspectionId == inspection.Id.Value
                select new WaterInspectionVesselDTO
                {
                    Id = vessel.Id,
                    Color = vessel.Color,
                    Length = vessel.Length,
                    Number = vessel.Number,
                    StorageLocation = vessel.StorageLocation,
                    TotalCount = vessel.TotalCount,
                    VesselTypeId = vessel.VesselTypeId,
                    Width = vessel.Width,
                    IsTaken = vessel.IsTaken ?? false,
                    IsStored = vessel.IsStored ?? false,
                }
            ).ToList();

            cwoDTO.Engines = (
                from engine in Db.InspectionEngines
                where engine.InspectionId == inspection.Id.Value
                select new WaterInspectionEngineDTO
                {
                    Id = engine.Id,
                    Model = engine.EngineModel,
                    Power = engine.EnginePower,
                    Type = engine.EngineType,
                    EngineDescription = engine.EngineDescription,
                    StorageLocation = engine.StorageLocation,
                    TotalCount = engine.TotalCount,
                    IsTaken = engine.IsTaken ?? false,
                    IsStored = engine.IsStored ?? false,
                }
            ).ToList();

            cwoDTO.Catches = GetCatchMeasures(inspection.Id.Value);

            return AssignFromBase(cwoDTO, inspection);
        }

        protected override void Submit(InspectionRegister inspDbEntry, InspectionCheckWaterObjectDTO cwoDTO)
        {
            WaterObjectCheck newCwoDbEntry = new()
            {
                Inspection = inspDbEntry,
                WaterObjectTypeId = cwoDTO.WaterObjectTypeId,
                WaterObjectCoordinates = cwoDTO.WaterObjectLocation != null
                    ? new Point(cwoDTO.WaterObjectLocation.Longitude.Value, cwoDTO.WaterObjectLocation.Latitude.Value)
                    : null,
                Name = cwoDTO.ObjectName,
                IsActive = true
            };

            if (cwoDTO.FishingGears != null)
            {
                foreach (WaterInspectionFishingGearDTO fishingGear in cwoDTO.FishingGears)
                {
                    FishingGearRegister gear = new()
                    {
                        PermitLicenseId = null,
                        FishingGearTypeId = fishingGear.TypeId,
                        GearCount = fishingGear.Count,
                        NetEyeSize = fishingGear.NetEyeSize,
                        HookCount = fishingGear.HookCount,
                        Length = fishingGear.Length,
                        Height = fishingGear.Height,
                        IsActive = true,
                        Description = fishingGear.Description,
                        HouseLength = fishingGear.HouseLength,
                        HouseWidth = fishingGear.HouseWidth,
                        Inspection = inspDbEntry,
                        TowelLength = fishingGear.TowelLength,
                    };
                    Db.FishingGearRegisters.Add(gear);

                    if (fishingGear.Marks != null)
                    {
                        foreach (FishingGearMarkDTO mark in fishingGear.Marks)
                        {
                            FishingGearMark entry = new()
                            {
                                FishingGear = gear,
                                MarkNum = mark.Number,
                                MarkStatusId = mark.StatusId,
                                Inspection = inspDbEntry,
                            };
                            Db.FishingGearMarks.Add(entry);
                        }
                    }

                    InspectedFishingGear inspectedGearDb = new()
                    {
                        Inspection = inspDbEntry,
                        InspectedFishingGearNavigation = gear,
                        IsStored = fishingGear.IsStored,
                        IsTaken = fishingGear.IsTaken,
                        StorageLocation = fishingGear.StorageLocation,
                        IsActive = true,
                    };
                    Db.InspectedFishingGears.Add(inspectedGearDb);
                }
            }

            if (cwoDTO.Vessels != null)
            {
                foreach (WaterInspectionVesselDTO vessel in cwoDTO.Vessels)
                {
                    Db.InspectionVessels.Add(new InspectionVessel
                    {
                        Color = vessel.Color,
                        IsStored = vessel.IsStored,
                        IsTaken = vessel.IsTaken,
                        StorageLocation = vessel.StorageLocation,
                        Length = vessel.Length,
                        Number = vessel.Number,
                        TotalCount = vessel.TotalCount,
                        Width = vessel.Width,
                        VesselTypeId = vessel.VesselTypeId,
                        Inspection = inspDbEntry,
                        IsActive = true,
                    });
                }
            }

            if (cwoDTO.Engines != null)
            {
                foreach (WaterInspectionEngineDTO engine in cwoDTO.Engines)
                {
                    Db.InspectionEngines.Add(new InspectionEngine
                    {
                        IsStored = engine.IsStored,
                        IsTaken = engine.IsTaken,
                        StorageLocation = engine.StorageLocation,
                        TotalCount = engine.TotalCount,
                        EngineModel = engine.Model,
                        EnginePower = engine.Power,
                        EngineType = engine.Type,
                        Inspection = inspDbEntry,
                        EngineDescription = engine.EngineDescription,
                        IsActive = true,
                    });
                }
            }

            AddCatchMeasures(inspDbEntry, cwoDTO.Catches);

            Db.WaterObjectChecks.Add(newCwoDbEntry);
        }
    }
}
