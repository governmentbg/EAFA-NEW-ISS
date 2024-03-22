using System;
using System.Collections.Generic;
using IARA.Flux.Models;
using IARA.Flux.Tests.FATests.Models;
using IARA.FluxModels.Enums;

namespace IARA.Flux.Tests.FATests
{
    public static class FaTests
    {
        /// <summary>
        /// 1. Тръгване с цел риболов (без риба на борда)
        /// 2. Една риболовна операция (пускане и вдигане на уред)
        /// 3. Предварително уведомление за връщане с риба на борда
        /// 4. Декларация за връщане с риба на борда
        /// 5. Декларация за разтоварване
        /// </summary>
        /// <returns></returns>
        public static List<FLUXFAReportMessageType> RegularTripOneFishingOperation()
        {
            DateTime departureTime = DateTime.Now;
            DateTime gearShotTime = departureTime.AddHours(2);
            DateTime gearRetrievalTime = gearShotTime.AddHours(4);
            DateTime arrivalNotificationTime = gearRetrievalTime.AddHours(2);
            DateTime arrivalDeclarationTime = gearRetrievalTime.AddHours(2).AddMinutes(10);
            DateTime landingStartTime = arrivalDeclarationTime.AddMinutes(30);
            DateTime landingEndTime = arrivalDeclarationTime.AddMinutes(55);

            FaTripTestBuilder builder = new();

            FaReportModel departureReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel arrivalNotificationReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel arrivalDeclarationReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel landingReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);

            FaDepartureModel departureModel = new()
            {
                Occurrence = departureTime,
                ReasonCode = "FIS",
                SpeciesTargetCode = "SPR",
                Locations = new FLUXLocationType[] { FaTripConstants.LocationPort1 },
                FishingGears = new FishingGearType[] { FaTripConstants.FishingGearOnboard11, FaTripConstants.FishingGearOnboard12 }
            };

            FaFishingOperationModel fishingOperationModel = new()
            {
                VesselRelatedActivityCode = "FIS",
                OperationsQuantity = 1,
                SpeciesTargetCode = "SPR",
                Catches = new FACatchType[]
                {
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "ONBOARD"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "TUR"),
                        UnitQuantity = new QuantityType{ unitCode = "C62", Value = 50 },
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 100),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed11 }
                    }
                },
                FishingGear = FaTripConstants.FishingGearDeployed11,
                StartTime = gearShotTime,
                EndTime = gearRetrievalTime
            };

            FaSubFishingOperationModel gearShotModel = new()
            {
                TypeCode = "GEAR_SHOT",
                Occurrence = gearShotTime,
                Location = FaTripConstants.LocationPosition1,
                FishingGear = FaTripConstants.FishingGearDeployed11,
                Characteristics = new FLUXCharacteristicType[]
                {
                    new FLUXCharacteristicType
                    {
                        TypeCode = CodeType.CreateCode("FA_CHARACTERISTIC", "FISHING_DEPTH"),
                        ValueMeasure = MeasureType.CreateMeasure(FluxUnits.MTR, 10)
                    }
                }
            };

            FaSubFishingOperationModel gearRetrievalModel = new()
            {
                TypeCode = "GEAR_RETRIEVAL",
                Occurrence = gearRetrievalTime,
                Location = FaTripConstants.LocationPosition1,
                FishingGear = FaTripConstants.FishingGearDeployed11,
                Characteristics = new FLUXCharacteristicType[]
                {
                    new FLUXCharacteristicType
                    {
                        TypeCode = CodeType.CreateCode("FA_CHARACTERISTIC", "FISHING_DEPTH"),
                        ValueMeasure = MeasureType.CreateMeasure(FluxUnits.MTR, 10)
                    }
                }
            };

            FaArrivalNotificationModel arrivalNotificationModel = new()
            {
                Occurrence = arrivalNotificationTime,
                ReasonCode = "LAN",
                TripStartDate = departureTime,
                TripEndDate = arrivalNotificationTime,
                Locations = new FLUXLocationType[] { FaTripConstants.LocationPort2 },
                Catches = new FACatchType[]
                {
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "UNLOADED"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "TUR"),
                        UnitQuantity = new QuantityType{ unitCode = "C62", Value = 50 },
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 100),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed11 }
                    }
                }
            };

            FaArrivalDeclarationModel arrivalDeclarationModel = new()
            {
                Occurrence = arrivalDeclarationTime,
                ReasonCode = "LAN",
                Locations = new FLUXLocationType[] { FaTripConstants.LocationPort2 },
                FishingGears = new FishingGearType[] { FaTripConstants.FishingGearOnboard11, FaTripConstants.FishingGearOnboard12 },
                Characteristics = new FLUXCharacteristicType[]
                {
                    new FLUXCharacteristicType
                    {
                        TypeCode = CodeType.CreateCode("FA_CHARACTERISTIC", "START_DATETIME_LANDING"),
                        ValueDateTime = landingStartTime
                    }
                }
            };

            FaLandingModel landingModel = new()
            {
                DateFrom = landingStartTime,
                DateTo = landingEndTime,
                Locations = new FLUXLocationType[] { FaTripConstants.LocationPort2 },
                Catches = new FACatchType[]
                {
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "UNLOADED"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "TUR"),
                        UnitQuantity = new QuantityType{ unitCode = "C62", Value = 50 },
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 102),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed11 },
                        AppliedAAPProcess = new AAPProcessType[]
                        {
                            new AAPProcessType
                            {
                                TypeCode = new CodeType[]
                                {
                                    CodeType.CreateCode("FISH_PRESENTATION", "WHL"),
                                    CodeType.CreateCode("FISH_PRESERVATION", "FRE"),
                                    CodeType.CreateCode("FISH_FRESHNESS", "E")
                                },
                                ConversionFactorNumeric = new NumericType{ Value = 1 },
                                ResultAAPProduct = new AAPProductType[]
                                {
                                    new AAPProductType
                                    {
                                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 102),
                                        PackagingUnitQuantity = new QuantityType{ unitCode = "C62", Value = 50 },
                                        PackagingTypeCode = CodeType.CreateCode("FISH_PACKAGING", "BOX")
                                    }
                                }
                            }
                        }
                    }
                }
            };

            FLUXFAReportMessageType result = builder.BuildDeparture(departureReport, departureModel);
            result = builder.BuildFishingOperation(fishingOperationReport, fishingOperationModel, result);

            result.FAReportDocument[^1].SpecifiedFishingActivity[0].RelatedFishingActivity = new FishingActivityType[]
            {
                builder.BuildSubFishingOperation(gearShotModel),
                builder.BuildSubFishingOperation(gearRetrievalModel)
            };

            result = builder.BuildArrivalNotification(arrivalNotificationReport, arrivalNotificationModel, result);
            result = builder.BuildArrivalDeclaration(arrivalDeclarationReport, arrivalDeclarationModel, result);
            result = builder.BuildLanding(landingReport, landingModel, result);

            return new List<FLUXFAReportMessageType> { result };
        }
    }
}
