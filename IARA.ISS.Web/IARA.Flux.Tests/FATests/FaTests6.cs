using System;
using System.Collections.Generic;
using IARA.Flux.Models;
using IARA.Flux.Tests.FATests.Models;
using IARA.FluxModels.Enums;

namespace IARA.Flux.Tests.FATests
{
    public static class FaTests6
    {
        /// <summary>
        /// 1. Тръгване
        /// 2. Пускане и вдигане на уред(250 кг барбун)
        /// 3. (Уведомление) и връщане
        /// 4. Разтоварване(250 кг цаца)
        /// 5. Корекция на разтоварване(240 кг барбун, 10 кг цаца)
        /// </summary>
        /// <returns></returns>
        public static List<FLUXFAReportMessageType> Test61RegularTripWithLandingCorrection()
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
            FaReportModel landingReportCorrection = new(FluxPurposes.Replace, null, null, null, FaTripConstants.Vessel1, null);

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
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 250),
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
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 250),
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

            FaLandingModel landingModelCorrection = new()
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
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 240),
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
                                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 240),
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
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 10),
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

            landingReportCorrection.ReferencedId = Guid.Parse(result.FAReportDocument[^1].RelatedFLUXReportDocument.ID[0].Value);
            result = builder.BuildLanding(landingReportCorrection, landingModelCorrection, result);

            return new List<FLUXFAReportMessageType> { result };
        }

        /// <summary>
        /// 1. Тръгване
        /// 2. Пускане- и вдигане на уред(10:00 пускане, 12:00 вдигане - 100 кг барбун)
        /// 3. Корекция на риболовната операция(10:30 пускане, 13:00 вдигане - 50 кг цаца, 50 кг барбун)
        /// 4. (Уведомление) и връщане
        /// 5. Разтоварване(100 кг цаца)
        /// </summary>
        /// <returns></returns>
        public static List<FLUXFAReportMessageType> Test62OneFoWithCorrection()
        {
            DateTime departureTime = DateTime.Now;
            DateTime gearShotTime = departureTime.AddHours(2);
            DateTime gearRetrievalTime = gearShotTime.AddHours(4);
            DateTime gearShotTimeCorrection = departureTime.AddHours(2).AddMinutes(30);
            DateTime gearRetrievalTimeCorrection = gearShotTime.AddHours(5);
            DateTime arrivalNotificationTime = gearRetrievalTimeCorrection.AddHours(2);
            DateTime arrivalDeclarationTime = gearRetrievalTimeCorrection.AddHours(2).AddMinutes(10);
            DateTime landingStartTime = arrivalDeclarationTime.AddMinutes(30);
            DateTime landingEndTime = arrivalDeclarationTime.AddMinutes(55);

            FaTripTestBuilder builder = new();

            FaReportModel departureReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReportCorrection = new(FluxPurposes.Replace, null, null, null, FaTripConstants.Vessel1, null);
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

            FaFishingOperationModel fishingOperationCorrectionModel = new()
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
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 50),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed11 }
                    },
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
                StartTime = gearShotTimeCorrection,
                EndTime = gearRetrievalTimeCorrection
            };

            FaSubFishingOperationModel gearShotCorrectionModel = new()
            {
                TypeCode = "GEAR_SHOT",
                Occurrence = gearShotTimeCorrection,
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

            FaSubFishingOperationModel gearRetrievalCorrectionModel = new()
            {
                TypeCode = "GEAR_RETRIEVAL",
                Occurrence = gearRetrievalTimeCorrection,
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
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 50),
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
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 49),
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
                                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 49),
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
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 51),
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
                                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 51),
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

            fishingOperationReportCorrection.ReferencedId = Guid.Parse(result.FAReportDocument[^1].RelatedFLUXReportDocument.ID[0].Value);
            result = builder.BuildFishingOperation(fishingOperationReportCorrection, fishingOperationCorrectionModel, result);

            result.FAReportDocument[^1].SpecifiedFishingActivity[0].RelatedFishingActivity = new FishingActivityType[]
            {
                builder.BuildSubFishingOperation(gearShotCorrectionModel),
                builder.BuildSubFishingOperation(gearRetrievalCorrectionModel)
            };

            result = builder.BuildArrivalNotification(arrivalNotificationReport, arrivalNotificationModel, result);
            result = builder.BuildArrivalDeclaration(arrivalDeclarationReport, arrivalDeclarationModel, result);
            result = builder.BuildLanding(landingReport, landingModel, result);

            return new List<FLUXFAReportMessageType> { result };
        }

        /// <summary>
        /// 1. Тръгване
        /// 2. Пускане и вдигане на уред(150 кг калкан)
        /// 3. Изхвърляне на 50 кг калкан
        /// 4. Корекция на изхвърлянето(100 кг калкан)
        /// 5. (Уведомление) и връщане
        /// 6. Разтоварване(50 кг калкан)
        /// </summary>
        /// <returns></returns>
        public static List<FLUXFAReportMessageType> Test63OneFoOneDiscardAndCorrectionOnDiscard()
        {
            DateTime departureTime = DateTime.Now;
            DateTime gearShotTime = departureTime.AddHours(2);
            DateTime gearRetrievalTime = gearShotTime.AddHours(4);
            DateTime discardTime = gearRetrievalTime.AddMinutes(30);
            DateTime discardTime2 = gearRetrievalTime.AddMinutes(30);
            DateTime arrivalNotificationTime = discardTime2.AddHours(2);
            DateTime arrivalDeclarationTime = discardTime2.AddHours(2).AddMinutes(10);
            DateTime landingStartTime = arrivalDeclarationTime.AddMinutes(30);
            DateTime landingEndTime = arrivalDeclarationTime.AddMinutes(55);

            FaTripTestBuilder builder = new();

            FaReportModel departureReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel discardReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel discardReportCorrection = new(FluxPurposes.Replace, null, null, null, FaTripConstants.Vessel1, null);
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
                        UnitQuantity =  new QuantityType{ unitCode = "C62", Value = 50 },
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 150),
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

            FaDiscardModel discardModel = new()
            {
                Occurrence = discardTime,
                ReasonCode = "PRO",
                Locations = new FLUXLocationType[] { FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                Catches = new FACatchType[]
                {
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "DISCARDED"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "TUR"),
                        UnitQuantity =  new QuantityType{ unitCode = "C62", Value = 20 },
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 50),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed11 }
                    }
                }
            };

            FaDiscardModel discardCorrectionModel = new()
            {
                Occurrence = discardTime2,
                ReasonCode = "PRO",
                Locations = new FLUXLocationType[] { FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                Catches = new FACatchType[]
                {
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "DISCARDED"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "TUR"),
                        UnitQuantity =  new QuantityType{ unitCode = "C62", Value = 35 },
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 100),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed11 }
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
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "ONBOARD"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "TUR"),
                        UnitQuantity =  new QuantityType{ unitCode = "C62", Value = 5 },
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 50),
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
                        UnitQuantity = new QuantityType{ unitCode = "C62", Value = 5 },
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
                                        PackagingUnitQuantity = new QuantityType{ unitCode = "C62", Value = 5 },
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

            discardReport.RelatedReportIds = new Guid[] { Guid.Parse(result.FAReportDocument[^1].RelatedFLUXReportDocument.ID[0].Value) };
            result = builder.BuildDiscard(discardReport, discardModel, result);

            discardReportCorrection.RelatedReportIds = new Guid[] { Guid.Parse(result.FAReportDocument[^2].RelatedFLUXReportDocument.ID[0].Value) };
            discardReportCorrection.ReferencedId = Guid.Parse(result.FAReportDocument[^1].RelatedFLUXReportDocument.ID[0].Value);
            result = builder.BuildDiscard(discardReportCorrection, discardCorrectionModel, result);

            result = builder.BuildArrivalNotification(arrivalNotificationReport, arrivalNotificationModel, result);
            result = builder.BuildArrivalDeclaration(arrivalDeclarationReport, arrivalDeclarationModel, result);
            result = builder.BuildLanding(landingReport, landingModel, result);

            return new List<FLUXFAReportMessageType> { result };
        }

        /// <summary>
        /// 1. Тръгване
        /// 2. Пускане и вдигане на уред(100 кг барбун)
        /// 3. Уведомление за връщане(14:00, Балчик, 100 кг цаца)
        /// 4. Корекция на уведомление(14:30, Каварна, 100 кг барбун)
        /// 5. Връщане(14:35, Каварна)
        /// 6. Разтоварване(100 кг барбун)
        /// </summary>
        /// <returns></returns>
        public static List<FLUXFAReportMessageType> Test64OneFoAndCorrectionOfArrivalNotification()
        {
            DateTime departureTime = DateTime.Now;
            DateTime gearShotTime = departureTime.AddHours(2);
            DateTime gearRetrievalTime = gearShotTime.AddHours(4);
            DateTime arrivalNotificationTime = gearRetrievalTime.AddHours(2);
            DateTime arrivalNotificationTime2 = gearRetrievalTime.AddHours(2).AddMinutes(10);
            DateTime arrivalDeclarationTime = gearRetrievalTime.AddHours(2).AddMinutes(20);
            DateTime landingStartTime = arrivalDeclarationTime.AddMinutes(30);
            DateTime landingEndTime = arrivalDeclarationTime.AddMinutes(55);

            FaTripTestBuilder builder = new();

            FaReportModel departureReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel arrivalNotificationReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel arrivalNotificationReportCorrection = new(FluxPurposes.Replace, null, null, null, FaTripConstants.Vessel1, null);
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
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "SPR"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 100),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed11 }
                    }
                }
            };

            FaArrivalNotificationModel arrivalNotificationCorrectionModel = new()
            {
                Occurrence = arrivalNotificationTime2,
                ReasonCode = "LAN",
                TripStartDate = departureTime,
                TripEndDate = arrivalNotificationTime2,
                Locations = new FLUXLocationType[] { FaTripConstants.LocationPort1 },
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

            FaArrivalDeclarationModel arrivalDeclarationModel = new()
            {
                Occurrence = arrivalDeclarationTime,
                ReasonCode = "LAN",
                Locations = new FLUXLocationType[] { FaTripConstants.LocationPort1 },
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
                Locations = new FLUXLocationType[] { FaTripConstants.LocationPort1 },
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
                                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 100),
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

            arrivalNotificationReportCorrection.ReferencedId = Guid.Parse(result.FAReportDocument[^1].RelatedFLUXReportDocument.ID[0].Value);
            result = builder.BuildArrivalNotification(arrivalNotificationReportCorrection, arrivalNotificationCorrectionModel, result);

            result = builder.BuildArrivalDeclaration(arrivalDeclarationReport, arrivalDeclarationModel, result);
            result = builder.BuildLanding(landingReport, landingModel, result);

            return new List<FLUXFAReportMessageType> { result };
        }

        /// <summary>
        /// 1. Тръгване
        /// 2. Пускане и вдигане на уред(150 кг цаца)
        /// 3. Уведомление за връщане 
        /// 4. Връщане в пристанище(14:00, Бургас)
        /// 5. Корекция на връщане(14:30, Созопол)
        /// 6. Разтоварване(150 кг цаца)
        /// </summary>
        /// <returns></returns>
        public static List<FLUXFAReportMessageType> Test65OneFoAndCorrectionOfArrivalDeclaration()
        {
            DateTime departureTime = DateTime.Now;
            DateTime gearShotTime = departureTime.AddHours(2);
            DateTime gearRetrievalTime = gearShotTime.AddHours(4);
            DateTime arrivalNotificationTime = gearRetrievalTime.AddHours(2);
            DateTime arrivalDeclarationTime = gearRetrievalTime.AddHours(2).AddMinutes(10);
            DateTime arrivalDeclarationTime2 = gearRetrievalTime.AddHours(2).AddMinutes(30);
            DateTime landingStartTime = arrivalDeclarationTime2.AddMinutes(30);
            DateTime landingEndTime = arrivalDeclarationTime2.AddMinutes(55);

            FaTripTestBuilder builder = new();

            FaReportModel departureReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel arrivalNotificationReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel arrivalDeclarationReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel arrivalDeclarationReportCorrection = new(FluxPurposes.Replace, null, null, null, FaTripConstants.Vessel1, null);
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
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 150),
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
                Locations = new FLUXLocationType[] { FaTripConstants.LocationPort1 },
                Catches = new FACatchType[]
                {
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

            FaArrivalDeclarationModel arrivalDeclarationCorrectionModel = new()
            {
                Occurrence = arrivalDeclarationTime2,
                ReasonCode = "LAN",
                Locations = new FLUXLocationType[] { FaTripConstants.LocationPort1 },
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
                Locations = new FLUXLocationType[] { FaTripConstants.LocationPort1 },
                Catches = new FACatchType[]
                {
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "UNLOADED"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "SPR"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 150),
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
                                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 150),
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

            arrivalDeclarationReportCorrection.ReferencedId = Guid.Parse(result.FAReportDocument[^1].RelatedFLUXReportDocument.ID[0].Value);
            result = builder.BuildArrivalDeclaration(arrivalDeclarationReportCorrection, arrivalDeclarationCorrectionModel, result);

            result = builder.BuildLanding(landingReport, landingModel, result);

            return new List<FLUXFAReportMessageType> { result };
        }

        /// <summary>
        /// 1. Тръгване (07:00, Несебър)
        ///2. Корекция на тръгване(07:20, Созопол)
        ///3. Пускане и вдигане на уред(100 кг цаца)
        ///4. (Уведомление) и връщане
        /// 5. Разтоварване(150 кг цаца)
        /// </summary>
        /// <returns></returns>
        public static List<FLUXFAReportMessageType> Test66OneFoAndCorrectionOfDeparture()
        {
            DateTime departureTime = DateTime.Now;
            DateTime departureTime2 = DateTime.Now.AddMinutes(30);
            DateTime gearShotTime = departureTime2.AddHours(2);
            DateTime gearRetrievalTime = gearShotTime.AddHours(4);
            DateTime arrivalNotificationTime = gearRetrievalTime.AddHours(2);
            DateTime arrivalDeclarationTime = gearRetrievalTime.AddHours(2).AddMinutes(10);
            DateTime landingStartTime = arrivalDeclarationTime.AddMinutes(30);
            DateTime landingEndTime = arrivalDeclarationTime.AddMinutes(55);

            FaTripTestBuilder builder = new();

            FaReportModel departureReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel departureReportCorrection = new(FluxPurposes.Replace, null, null, null, FaTripConstants.Vessel1, null);
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

            FaDepartureModel departureCorrectionModel = new()
            {
                Occurrence = departureTime2,
                ReasonCode = "FIS",
                SpeciesTargetCode = "SPR",
                Locations = new FLUXLocationType[] { FaTripConstants.LocationPort2 },
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
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 100),
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
                                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 100),
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

            departureReportCorrection.ReferencedId = Guid.Parse(result.FAReportDocument[^1].RelatedFLUXReportDocument.ID[0].Value);
            result = builder.BuildDeparture(departureReportCorrection, departureCorrectionModel, result);

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
