using IARA.Flux.Models;
using IARA.Flux.Tests.FATests.Models;
using IARA.FluxModels.Enums;
using System;
using System.Collections.Generic;

namespace IARA.Flux.Tests.FATests
{
    public static class FaTests7
    {
        /// <summary>
        /// 1. Тръгване
        /// 2. Пускане и вдигане на уред(100 кг барбун)
        /// 3. Уведомление за връщане(15:00)
        /// 4. Отмяна на уведомлението за връщане
        /// 5. Пускане и вдигане на уред(150 кг барбун)
        /// 6. Уведомление за връщане(20:00)
        /// 7. Връщане(20:05)
        /// 8. Разтоварване(250 кг барбун)
        /// </summary>
        /// <returns></returns>
        public static List<FLUXFAReportMessageType> Test71TwoFoCancellationOfArrivalNotification()
        {
            DateTime departureTime = DateTime.Now;
            DateTime gearShotTime = departureTime.AddHours(2);
            DateTime gearRetrievalTime = gearShotTime.AddHours(4);
            DateTime arrivalNotificationTime = gearRetrievalTime.AddHours(2);
            DateTime gearShotTime2 = gearRetrievalTime.AddHours(2);
            DateTime gearRetrievalTime2 = gearRetrievalTime.AddHours(4);
            DateTime arrivalNotificationTime2 = gearRetrievalTime2.AddHours(2);
            DateTime arrivalDeclarationTime = gearRetrievalTime2.AddHours(2).AddMinutes(10);
            DateTime landingStartTime = arrivalDeclarationTime.AddMinutes(30);
            DateTime landingEndTime = arrivalDeclarationTime.AddMinutes(55);

            FaTripTestBuilder builder = new();

            FaReportModel departureReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel arrivalNotificationReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel arrivalNotificationReportCancellation = new(FluxPurposes.Cancellation, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport2 = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel arrivalNotificationReport2 = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel arrivalDeclarationReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel landingReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);

            FaDepartureModel departureModel = new()
            {
                Occurrence = departureTime,
                ReasonCode = "FIS",
                SpeciesTargetCode = "MUT",
                Locations = new FLUXLocationType[] { FaTripConstants.LocationPort1 },
                FishingGears = new FishingGearType[] { FaTripConstants.FishingGearOnboard11, FaTripConstants.FishingGearOnboard12 }
            };

            FaFishingOperationModel fishingOperationModel = new()
            {
                VesselRelatedActivityCode = "FIS",
                OperationsQuantity = 1,
                SpeciesTargetCode = "MUT",
                Catches = new FACatchType[]
                {
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "ONBOARD"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "MUT"),
                        UnitQuantity = null,
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
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "MUT"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 100),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed11 }
                    }
                }
            };

            FaFishingOperationModel fishingOperationModel2 = new()
            {
                VesselRelatedActivityCode = "FIS",
                OperationsQuantity = 1,
                SpeciesTargetCode = "MUT",
                Catches = new FACatchType[]
    {
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "ONBOARD"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "MUT"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 150),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed11 }
                    }
    },
                FishingGear = FaTripConstants.FishingGearDeployed11,
                StartTime = gearShotTime2,
                EndTime = gearRetrievalTime2
            };

            FaSubFishingOperationModel gearShotModel2 = new()
            {
                TypeCode = "GEAR_SHOT",
                Occurrence = gearShotTime2,
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

            FaSubFishingOperationModel gearRetrievalModel2 = new()
            {
                TypeCode = "GEAR_RETRIEVAL",
                Occurrence = gearRetrievalTime2,
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

            FaArrivalNotificationModel arrivalNotificationModel2 = new()
            {
                Occurrence = arrivalNotificationTime2,
                ReasonCode = "LAN",
                TripStartDate = departureTime,
                TripEndDate = arrivalNotificationTime2,
                Locations = new FLUXLocationType[] { FaTripConstants.LocationPort2 },
                Catches = new FACatchType[]
                {
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "UNLOADED"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "MUT"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 100),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed11 }
                    },
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "UNLOADED"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "MUT"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 150),
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
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "MUT"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 250),
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
                                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 250),
                                        PackagingUnitQuantity = null,
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

            arrivalNotificationReportCancellation.ReferencedId = Guid.Parse(result.FAReportDocument[^1].RelatedFLUXReportDocument.ID[0].Value);
            result = builder.BuildArrivalNotification(arrivalNotificationReportCancellation, null, result);

            result = builder.BuildFishingOperation(fishingOperationReport2, fishingOperationModel2, result);

            result.FAReportDocument[^1].SpecifiedFishingActivity[0].RelatedFishingActivity = new FishingActivityType[]
            {
                builder.BuildSubFishingOperation(gearShotModel2),
                builder.BuildSubFishingOperation(gearRetrievalModel2)
            };

            result = builder.BuildArrivalNotification(arrivalNotificationReport2, arrivalNotificationModel2, result);

            result = builder.BuildArrivalDeclaration(arrivalDeclarationReport, arrivalDeclarationModel, result);
            result = builder.BuildLanding(landingReport, landingModel, result);

            return new List<FLUXFAReportMessageType> { result };
        }
    }
}
