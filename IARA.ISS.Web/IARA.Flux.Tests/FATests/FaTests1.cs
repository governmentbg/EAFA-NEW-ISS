using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Flux.Models;
using IARA.Flux.Tests.FATests.Models;
using IARA.FluxModels.Enums;

namespace IARA.Flux.Tests.FATests
{
    public static class FaTests1
    {
        /// <summary>
        /// 1. Тръгване
        /// 2. Пускане и вдигане на уред
        /// 3. (Уведомление) и връщане
        /// 4. Разтоварване
        /// </summary>
        /// <returns></returns>
        public static List<FLUXFAReportMessageType> Test11RegularTripOneFo()
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
                SpeciesTargetCode = "TUR",
                Locations = new FLUXLocationType[] { FaTripConstants.LocationPort1 },
                FishingGears = new FishingGearType[] { FaTripConstants.FishingGearOnboard11, FaTripConstants.FishingGearOnboard12 }
            };

            FaFishingOperationModel fishingOperationModel = new()
            {
                VesselRelatedActivityCode = "FIS",
                OperationsQuantity = 1,
                SpeciesTargetCode = "TUR",
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

        /// <summary>
        /// 1. Тръгване
        /// 2. Пускане и вдигане на уред(100 кг цаца)
        /// 3. (Уведомление) и връщане
        /// 4. Разтоварване(90 кг цаца, 10 кг барбун)
        /// </summary>
        /// <returns></returns>
        public static List<FLUXFAReportMessageType> Test12RegularTripOneFoDifferenceInLanding()
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
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "SPR"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 100),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition2, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed12 }
                    }
                },
                FishingGear = FaTripConstants.FishingGearDeployed12,
                StartTime = gearShotTime,
                EndTime = gearRetrievalTime
            };

            FaSubFishingOperationModel gearShotModel = new()
            {
                TypeCode = "GEAR_SHOT",
                Occurrence = gearShotTime,
                Location = FaTripConstants.LocationPosition1,
                FishingGear = FaTripConstants.FishingGearDeployed12,
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
                FishingGear = FaTripConstants.FishingGearDeployed12,
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
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "SPR"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 100),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed12 }
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
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "SPR"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 90),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed12 },
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
                                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 90),
                                        PackagingUnitQuantity = null,
                                        PackagingTypeCode = CodeType.CreateCode("FISH_PACKAGING", "BOX")
                                    }
                                }
                            }
                        }
                    },
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "UNLOADED"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "MUT"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 10),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed12 },
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
                                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 10),
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
            result = builder.BuildArrivalDeclaration(arrivalDeclarationReport, arrivalDeclarationModel, result);
            result = builder.BuildLanding(landingReport, landingModel, result);

            return new List<FLUXFAReportMessageType> { result };
        }

        /// <summary>
        /// 1. Тръгване
        /// 2. Пускане и вдигане на уред(0 кг барбун)
        /// 3. (Уведомление) и връщане
        /// 4. Празна декларация
        /// </summary>
        /// <returns></returns>
        public static List<FLUXFAReportMessageType> Test13RegularTripOneFoNoQuantitiesInLanding()
        {
            DateTime departureTime = DateTime.Now;
            DateTime gearShotTime = departureTime.AddHours(2);
            DateTime gearRetrievalTime = gearShotTime.AddHours(4);
            DateTime arrivalNotificationTime = gearRetrievalTime.AddHours(2);
            DateTime arrivalDeclarationTime = gearRetrievalTime.AddHours(2).AddMinutes(10);
            DateTime landingStartTime = arrivalDeclarationTime.AddMinutes(30);
            DateTime landingEndTime = arrivalDeclarationTime.AddMinutes(32);

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
                SpeciesTargetCode = "MUT",
                Locations = new FLUXLocationType[] { FaTripConstants.LocationPort1 },
                FishingGears = new FishingGearType[] { FaTripConstants.FishingGearOnboard11, FaTripConstants.FishingGearOnboard12 }
            };

            FaFishingOperationModel fishingOperationModel = new()
            {
                VesselRelatedActivityCode = "FIS",
                OperationsQuantity = 1,
                SpeciesTargetCode = "MUT",
                FishingGear = FaTripConstants.FishingGearDeployed12,
                StartTime = gearShotTime,
                EndTime = gearRetrievalTime
            };

            FaSubFishingOperationModel gearShotModel = new()
            {
                TypeCode = "GEAR_SHOT",
                Occurrence = gearShotTime,
                Location = FaTripConstants.LocationPosition1,
                FishingGear = FaTripConstants.FishingGearDeployed12,
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
                FishingGear = FaTripConstants.FishingGearDeployed12,
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

        /// <summary>
        /// 1. Тръгване
        /// 2. Пускане и вдигане на уред(100 кг цаца)
        /// 3. Пускане и вдигане на уред(150 кг цаца)
        /// 4. Пускане и вдигане на уред(200 кг цаца)
        /// 5. (Уведомление) и връщане
        /// 6. Разтоварване с разлики(443 кг цаца)
        /// </summary>
        /// <returns></returns>
        public static List<FLUXFAReportMessageType> Test14RegularTripThreeFoOneGearSummedLanding()
        {
            DateTime departureTime = DateTime.Now;
            DateTime gearShotTime = departureTime.AddHours(2);
            DateTime gearRetrievalTime = gearShotTime.AddHours(3).AddMinutes(20);
            DateTime gearShotTime2 = gearRetrievalTime.AddHours(1).AddMinutes(15);
            DateTime gearRetrievalTime2 = gearShotTime2.AddHours(1).AddMinutes(22);
            DateTime gearShotTime3 = gearRetrievalTime2.AddHours(1).AddMinutes(38);
            DateTime gearRetrievalTime3 = gearShotTime3.AddHours(1).AddMinutes(43);
            DateTime arrivalNotificationTime = gearRetrievalTime3.AddHours(2);
            DateTime arrivalDeclarationTime = gearRetrievalTime3.AddHours(2).AddMinutes(10);
            DateTime landingStartTime = arrivalDeclarationTime.AddMinutes(30);
            DateTime landingEndTime = arrivalDeclarationTime.AddMinutes(55);

            FaTripTestBuilder builder = new();

            FaReportModel departureReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport2 = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport3 = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
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
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "SPR"),
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

            FaFishingOperationModel fishingOperation2Model = new()
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
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 50),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed11 }
                    }
                },
                FishingGear = FaTripConstants.FishingGearDeployed11,
                StartTime = gearShotTime2,
                EndTime = gearRetrievalTime2
            };

            FaSubFishingOperationModel gearShot2Model = new()
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
                        ValueMeasure = MeasureType.CreateMeasure(FluxUnits.MTR, 15)
                    }
                }
            };

            FaSubFishingOperationModel gearRetrieval2Model = new()
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
                        ValueMeasure = MeasureType.CreateMeasure(FluxUnits.MTR, 15)
                    }
                }
            };

            FaFishingOperationModel fishingOperation3Model = new()
            {
                VesselRelatedActivityCode = "FIS",
                OperationsQuantity = 1,
                SpeciesTargetCode = "SPR",
                Catches = new FACatchType[]
                {
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "ONBOARD"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "SPR"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 150),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed11 }
                    }
                },
                FishingGear = FaTripConstants.FishingGearDeployed11,
                StartTime = gearShotTime3,
                EndTime = gearRetrievalTime3
            };

            FaSubFishingOperationModel gearShot3Model = new()
            {
                TypeCode = "GEAR_SHOT",
                Occurrence = gearShotTime3,
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

            FaSubFishingOperationModel gearRetrieval3Model = new()
            {
                TypeCode = "GEAR_RETRIEVAL",
                Occurrence = gearRetrievalTime3,
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
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "SPR"),
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
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 50),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed11 }
                    },
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "UNLOADED"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "SPR"),
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
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "SPR"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 101),
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
                                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 101),
                                        PackagingUnitQuantity = null,
                                        PackagingTypeCode = CodeType.CreateCode("FISH_PACKAGING", "BOX")
                                    }
                                }
                            }
                        }
                    },
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "UNLOADED"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "MUT"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 50),
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
                                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 50),
                                        PackagingUnitQuantity = null,
                                        PackagingTypeCode = CodeType.CreateCode("FISH_PACKAGING", "BOX")
                                    }
                                }
                            }
                        }
                    },
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "UNLOADED"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "SPR"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 149),
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
                                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 149),
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

            result = builder.BuildFishingOperation(fishingOperationReport2, fishingOperation2Model, result);

            result.FAReportDocument[^1].SpecifiedFishingActivity[0].RelatedFishingActivity = new FishingActivityType[]
            {
                builder.BuildSubFishingOperation(gearShot2Model),
                builder.BuildSubFishingOperation(gearRetrieval2Model)
            };

            result = builder.BuildFishingOperation(fishingOperationReport3, fishingOperation3Model, result);

            result.FAReportDocument[^1].SpecifiedFishingActivity[0].RelatedFishingActivity = new FishingActivityType[]
            {
                builder.BuildSubFishingOperation(gearShot3Model),
                builder.BuildSubFishingOperation(gearRetrieval3Model)
            };

            result = builder.BuildArrivalNotification(arrivalNotificationReport, arrivalNotificationModel, result);
            result = builder.BuildArrivalDeclaration(arrivalDeclarationReport, arrivalDeclarationModel, result);
            result = builder.BuildLanding(landingReport, landingModel, result);

            return new List<FLUXFAReportMessageType> { result };
        }

        /// <summary>
        /// 1. Тръгване
        /// 2. Пускане и вдигане на уред 1 (100 кг цаца)
        /// 3. Пускане и вдигане на уред 2 (50 кг барбун)
        /// 4. Пускане и вдигане на уред 1 (150 кг цаца)
        /// 5. (Уведомление) и връщане
        /// 6. Разтоварване с разлики(101 кг цаца, 50 кг барбун, 149 кг цаца)
        /// </summary>
        /// <returns></returns>
        public static List<FLUXFAReportMessageType> Test15RegularTripThreeFoTwoGears()
        {
            DateTime departureTime = DateTime.Now;
            DateTime gearShotTime = departureTime.AddHours(2);
            DateTime gearRetrievalTime = gearShotTime.AddHours(3).AddMinutes(20);
            DateTime gearShotTime2 = gearRetrievalTime.AddHours(1).AddMinutes(15);
            DateTime gearRetrievalTime2 = gearShotTime2.AddHours(1).AddMinutes(22);
            DateTime gearShotTime3 = gearRetrievalTime2.AddHours(1).AddMinutes(38);
            DateTime gearRetrievalTime3 = gearShotTime3.AddHours(7).AddMinutes(43);
            DateTime arrivalNotificationTime = gearRetrievalTime3.AddHours(2);
            DateTime arrivalDeclarationTime = gearRetrievalTime3.AddHours(2).AddMinutes(10);
            DateTime landingStartTime = arrivalDeclarationTime.AddMinutes(30);
            DateTime landingEndTime = arrivalDeclarationTime.AddMinutes(55);

            FaTripTestBuilder builder = new();

            FaReportModel departureReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport2 = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport3 = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
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
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "SPR"),
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

            FaFishingOperationModel fishingOperation2Model = new()
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
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 50),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed12 }
                    }
                },
                FishingGear = FaTripConstants.FishingGearDeployed12,
                StartTime = gearShotTime2,
                EndTime = gearRetrievalTime2
            };

            FaSubFishingOperationModel gearShot2Model = new()
            {
                TypeCode = "GEAR_SHOT",
                Occurrence = gearShotTime2,
                Location = FaTripConstants.LocationPosition1,
                FishingGear = FaTripConstants.FishingGearDeployed12,
                Characteristics = new FLUXCharacteristicType[]
                {
                    new FLUXCharacteristicType
                    {
                        TypeCode = CodeType.CreateCode("FA_CHARACTERISTIC", "FISHING_DEPTH"),
                        ValueMeasure = MeasureType.CreateMeasure(FluxUnits.MTR, 15)
                    }
                }
            };

            FaSubFishingOperationModel gearRetrieval2Model = new()
            {
                TypeCode = "GEAR_RETRIEVAL",
                Occurrence = gearRetrievalTime2,
                Location = FaTripConstants.LocationPosition1,
                FishingGear = FaTripConstants.FishingGearDeployed12,
                Characteristics = new FLUXCharacteristicType[]
                {
                    new FLUXCharacteristicType
                    {
                        TypeCode = CodeType.CreateCode("FA_CHARACTERISTIC", "FISHING_DEPTH"),
                        ValueMeasure = MeasureType.CreateMeasure(FluxUnits.MTR, 15)
                    }
                }
            };

            FaFishingOperationModel fishingOperation3Model = new()
            {
                VesselRelatedActivityCode = "FIS",
                OperationsQuantity = 1,
                SpeciesTargetCode = "SPR",
                Catches = new FACatchType[]
                {
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "ONBOARD"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "SPR"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 150),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed11 }
                    }
                },
                FishingGear = FaTripConstants.FishingGearDeployed11,
                StartTime = gearShotTime3,
                EndTime = gearRetrievalTime3
            };

            FaSubFishingOperationModel gearShot3Model = new()
            {
                TypeCode = "GEAR_SHOT",
                Occurrence = gearShotTime3,
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

            FaSubFishingOperationModel gearRetrieval3Model = new()
            {
                TypeCode = "GEAR_RETRIEVAL",
                Occurrence = gearRetrievalTime3,
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
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "SPR"),
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
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 50),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed12 }
                    },
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "UNLOADED"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "SPR"),
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
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "SPR"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 101),
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
                                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 101),
                                        PackagingUnitQuantity = null,
                                        PackagingTypeCode = CodeType.CreateCode("FISH_PACKAGING", "BOX")
                                    }
                                }
                            }
                        }
                    },
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "UNLOADED"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "MUT"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 50),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed12 },
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
                                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 50),
                                        PackagingUnitQuantity = null,
                                        PackagingTypeCode = CodeType.CreateCode("FISH_PACKAGING", "BOX")
                                    }
                                }
                            }
                        }
                    },
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "UNLOADED"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "SPR"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 149),
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
                                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 169),
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

            result = builder.BuildFishingOperation(fishingOperationReport2, fishingOperation2Model, result);

            result.FAReportDocument[^1].SpecifiedFishingActivity[0].RelatedFishingActivity = new FishingActivityType[]
            {
                builder.BuildSubFishingOperation(gearShot2Model),
                builder.BuildSubFishingOperation(gearRetrieval2Model)
            };

            result = builder.BuildFishingOperation(fishingOperationReport3, fishingOperation3Model, result);

            result.FAReportDocument[^1].SpecifiedFishingActivity[0].RelatedFishingActivity = new FishingActivityType[]
            {
                builder.BuildSubFishingOperation(gearShot3Model),
                builder.BuildSubFishingOperation(gearRetrieval3Model)
            };

            result = builder.BuildArrivalNotification(arrivalNotificationReport, arrivalNotificationModel, result);
            result = builder.BuildArrivalDeclaration(arrivalDeclarationReport, arrivalDeclarationModel, result);
            result = builder.BuildLanding(landingReport, landingModel, result);

            return new List<FLUXFAReportMessageType> { result };
        }
    }
}
