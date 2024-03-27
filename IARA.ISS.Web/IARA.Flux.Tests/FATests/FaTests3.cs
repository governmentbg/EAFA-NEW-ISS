using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Flux.Models;
using IARA.Flux.Tests.FATests.Models;
using IARA.FluxModels.Enums;

namespace IARA.Flux.Tests.FATests
{
    public static class FaTests3
    {
        /// <summary>
        /// Рейс 1:
        /// 1. Тръгване
        /// 2. Пускане на уред(няма улов)
        /// 3. (Уведомление) и връщане
        /// 4. Празна декларация за произход
        /// 
        /// Рейс 2:
        /// 1. Тръгване
        /// 2. Вдигане на уред(125 кг калкан)
        /// 3. (Уведомление) и връщане
        /// 4. Разтоварване(125 кг калкан)
        /// </summary>
        /// <returns></returns>
        public static List<FLUXFAReportMessageType> Test31GearDownAndUpInTwoTrips()
        {
            FLUXFAReportMessageType trip1 = Test31GearDownAndUpInTwoTripsTrip1(DateTime.Now)[0];
            FLUXFAReportMessageType trip2 = Test31GearDownAndUpInTwoTripsTrip2(DateTime.Now.AddDays(1).AddHours(3))[0];

            FAReportDocumentType gearShot = trip1.FAReportDocument.First(x => x.SpecifiedFishingActivity[0].TypeCode.Value == "FISHING_OPERATION");
            FAReportDocumentType gearRetrieval = trip2.FAReportDocument.First(x => x.SpecifiedFishingActivity[0].TypeCode.Value == "FISHING_OPERATION");

            Guid gearShotGuid = Guid.Parse(gearShot.RelatedFLUXReportDocument.ID[0].Value);
            gearRetrieval.RelatedReportID = new IDType[] { IDType.CreateFromGuid(gearShotGuid) };

            return new List<FLUXFAReportMessageType> { trip1, trip2 };
        }

        private static List<FLUXFAReportMessageType> Test31GearDownAndUpInTwoTripsTrip1(DateTime departureTime)
        {
            DateTime gearShotTime = departureTime.AddHours(2);
            DateTime arrivalNotificationTime = gearShotTime.AddHours(2);
            DateTime arrivalDeclarationTime = gearShotTime.AddHours(2).AddMinutes(10);
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
                SpeciesTargetCode = "TUR",
                Locations = new FLUXLocationType[] { FaTripConstants.LocationPort1 },
                FishingGears = new FishingGearType[] { FaTripConstants.FishingGearOnboard11, FaTripConstants.FishingGearOnboard12 }
            };

            FaFishingOperationModel fishingOperationModel = new()
            {
                VesselRelatedActivityCode = "SET",
                OperationsQuantity = 1,
                SpeciesTargetCode = "TUR",
                FishingGear = FaTripConstants.FishingGearDeployed11
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

            FaArrivalNotificationModel arrivalNotificationModel = new()
            {
                Occurrence = arrivalNotificationTime,
                ReasonCode = "LAN",
                TripStartDate = departureTime,
                TripEndDate = arrivalNotificationTime,
                Locations = new FLUXLocationType[] { FaTripConstants.LocationPort2 }
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
                Catches = new FACatchType[] { }
            };

            FLUXFAReportMessageType result = builder.BuildDeparture(departureReport, departureModel);
            result = builder.BuildFishingOperation(fishingOperationReport, fishingOperationModel, result);

            result.FAReportDocument[^1].SpecifiedFishingActivity[0].RelatedFishingActivity = new FishingActivityType[]
            {
                builder.BuildSubFishingOperation(gearShotModel)
            };

            result = builder.BuildArrivalNotification(arrivalNotificationReport, arrivalNotificationModel, result);
            result = builder.BuildArrivalDeclaration(arrivalDeclarationReport, arrivalDeclarationModel, result);
            result = builder.BuildLanding(landingReport, landingModel, result);

            return new List<FLUXFAReportMessageType> { result };
        }

        private static List<FLUXFAReportMessageType> Test31GearDownAndUpInTwoTripsTrip2(DateTime departureTime)
        {
            DateTime gearRetrievalTime = departureTime.AddHours(2);
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
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 125),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed11 }
                    }
                },
                FishingGear = FaTripConstants.FishingGearDeployed11
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
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 125),
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
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 125),
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
                                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 125),
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
                builder.BuildSubFishingOperation(gearRetrievalModel)
            };

            result = builder.BuildArrivalNotification(arrivalNotificationReport, arrivalNotificationModel, result);
            result = builder.BuildArrivalDeclaration(arrivalDeclarationReport, arrivalDeclarationModel, result);
            result = builder.BuildLanding(landingReport, landingModel, result);

            return new List<FLUXFAReportMessageType> { result };
        }

        /// <summary>
        /// Рейс 1: 
        /// 1. Тръгване
        /// 2. Пускане на уред 1 (няма улов)
        /// 3. Пускане на уред 2 (няма улов)
        /// 4. (Уведомление) и връщане
        /// 5. Празна декларация за произход
        /// 
        /// Рейс 2:
        /// 1. Тръгване
        /// 2. Вдигане на уред 1 (50 кг барбун)
        /// 3. Вдигане на уред 2 (75 кг барбун, 25 кг цаца)
        /// 4. Пускане на уред 1 (няма улов)
        /// 5. (Уведомление) и връщане
        /// 6. Разтоварване(50 кг барбун, 75 кг барбун, 25 кг цаца)

        /// Рейс 3:
        /// 1. Тръгване
        /// 2. Вдигане на уред 1 (150 кг цаца)
        /// 3. (Уведомление) и връщане
        /// 4. Разтоварване(150 кг цаца)
        /// </summary>
        /// <returns></returns>
        public static List<FLUXFAReportMessageType> Test32TwoGearDownAndUpInThreeTrips()
        {
            FLUXFAReportMessageType trip1 = Test32TwoGearDownAndUpInThreeTripsTrip1(DateTime.Now)[0];
            FLUXFAReportMessageType trip2 = Test32TwoGearDownAndUpInThreeTripsTrip2(DateTime.Now.AddDays(1).AddHours(3))[0];
            FLUXFAReportMessageType trip3 = Test32TwoGearDownAndUpInThreeTripsTrip3(DateTime.Now.AddDays(2).AddHours(2))[0];

            FAReportDocumentType trip1GearShot1 = trip1.FAReportDocument.First(x => x.SpecifiedFishingActivity[0].TypeCode.Value == "FISHING_OPERATION");
            FAReportDocumentType trip1GearShot2 = trip1.FAReportDocument.Last(x => x.SpecifiedFishingActivity[0].TypeCode.Value == "FISHING_OPERATION");

            Guid trip1GearShot1Guid = Guid.Parse(trip1GearShot1.RelatedFLUXReportDocument.ID[0].Value);
            Guid trip1GearShot2Guid = Guid.Parse(trip1GearShot2.RelatedFLUXReportDocument.ID[0].Value);

            FAReportDocumentType trip2GearRetrieval1 = trip2.FAReportDocument
                .Where(x => x.SpecifiedFishingActivity[0].TypeCode.Value == "FISHING_OPERATION")
                .ToList()[0];

            trip2GearRetrieval1.RelatedReportID = new IDType[] { IDType.CreateFromGuid(trip1GearShot1Guid) };

            FAReportDocumentType trip2GearRetrieval2 = trip2.FAReportDocument
                .Where(x => x.SpecifiedFishingActivity[0].TypeCode.Value == "FISHING_OPERATION")
                .ToList()[1];

            trip2GearRetrieval2.RelatedReportID = new IDType[] { IDType.CreateFromGuid(trip1GearShot2Guid) };

            FAReportDocumentType trip2GearShot = trip2.FAReportDocument
                .Where(x => x.SpecifiedFishingActivity[0].TypeCode.Value == "FISHING_OPERATION")
                .ToList()[2];

            Guid trip2GearShotGuid = Guid.Parse(trip2GearShot.RelatedFLUXReportDocument.ID[0].Value);

            FAReportDocumentType trip3GearRetrieval = trip3.FAReportDocument.First(x => x.SpecifiedFishingActivity[0].TypeCode.Value == "FISHING_OPERATION");

            trip3GearRetrieval.RelatedReportID = new IDType[] { IDType.CreateFromGuid(trip2GearShotGuid) };

            return new List<FLUXFAReportMessageType> { trip1, trip2, trip3 };
        }

        private static List<FLUXFAReportMessageType> Test32TwoGearDownAndUpInThreeTripsTrip1(DateTime departureTime)
        {
            DateTime gearShotTime = departureTime.AddHours(2);
            DateTime gearShotTime2 = gearShotTime.AddHours(1);
            DateTime arrivalNotificationTime = gearShotTime2.AddHours(2);
            DateTime arrivalDeclarationTime = gearShotTime2.AddHours(2).AddMinutes(10);
            DateTime landingStartTime = arrivalDeclarationTime.AddMinutes(30);
            DateTime landingEndTime = arrivalDeclarationTime.AddMinutes(32);

            FaTripTestBuilder builder = new();

            FaReportModel departureReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport1 = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport2 = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
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

            FaFishingOperationModel fishingOperationModel1 = new()
            {
                VesselRelatedActivityCode = "SET",
                OperationsQuantity = 1,
                SpeciesTargetCode = "MUT",
                FishingGear = FaTripConstants.FishingGearDeployed11
            };

            FaSubFishingOperationModel gearShotModel1 = new()
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

            FaFishingOperationModel fishingOperationModel2 = new()
            {
                VesselRelatedActivityCode = "SET",
                OperationsQuantity = 1,
                SpeciesTargetCode = "MUT",
                FishingGear = FaTripConstants.FishingGearDeployed12
            };

            FaSubFishingOperationModel gearShotModel2 = new()
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
                Locations = new FLUXLocationType[] { FaTripConstants.LocationPort2 }
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
                Catches = new FACatchType[] { }
            };

            FLUXFAReportMessageType result = builder.BuildDeparture(departureReport, departureModel);
            result = builder.BuildFishingOperation(fishingOperationReport1, fishingOperationModel1, result);

            result.FAReportDocument[^1].SpecifiedFishingActivity[0].RelatedFishingActivity = new FishingActivityType[]
            {
                builder.BuildSubFishingOperation(gearShotModel1)
            };

            result = builder.BuildFishingOperation(fishingOperationReport2, fishingOperationModel2, result);

            result.FAReportDocument[^1].SpecifiedFishingActivity[0].RelatedFishingActivity = new FishingActivityType[]
            {
                builder.BuildSubFishingOperation(gearShotModel2)
            };

            result = builder.BuildArrivalNotification(arrivalNotificationReport, arrivalNotificationModel, result);
            result = builder.BuildArrivalDeclaration(arrivalDeclarationReport, arrivalDeclarationModel, result);
            result = builder.BuildLanding(landingReport, landingModel, result);

            return new List<FLUXFAReportMessageType> { result };
        }

        private static List<FLUXFAReportMessageType> Test32TwoGearDownAndUpInThreeTripsTrip2(DateTime departureTime)
        {
            DateTime gearRetrievalTime1 = departureTime.AddHours(2);
            DateTime gearRetrievalTime2 = gearRetrievalTime1.AddHours(1);
            DateTime gearShotTime = gearRetrievalTime2.AddHours(1);
            DateTime arrivalNotificationTime = gearShotTime.AddHours(2);
            DateTime arrivalDeclarationTime = gearShotTime.AddHours(2).AddMinutes(10);
            DateTime landingStartTime = arrivalDeclarationTime.AddMinutes(30);
            DateTime landingEndTime = arrivalDeclarationTime.AddMinutes(55);

            FaTripTestBuilder builder = new();

            FaReportModel departureReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport1 = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport2 = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport3 = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
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

            FaFishingOperationModel fishingOperationModel1 = new()
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
                FishingGear = FaTripConstants.FishingGearDeployed11
            };

            FaSubFishingOperationModel gearRetrievalModel1 = new()
            {
                TypeCode = "GEAR_RETRIEVAL",
                Occurrence = gearRetrievalTime1,
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
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 75),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed12 }
                    },
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "ONBOARD"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "SPR"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 25),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed12 }
                    }
                },
                FishingGear = FaTripConstants.FishingGearDeployed12
            };

            FaSubFishingOperationModel gearRetrievalModel2 = new()
            {
                TypeCode = "GEAR_RETRIEVAL",
                Occurrence = gearRetrievalTime1,
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

            FaFishingOperationModel fishingOperationModel3 = new()
            {
                VesselRelatedActivityCode = "SET",
                OperationsQuantity = 1,
                SpeciesTargetCode = "SPR",
                FishingGear = FaTripConstants.FishingGearDeployed11
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
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 75),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed12 }
                    },
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "UNLOADED"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "SPR"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 25),
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
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "MUT"),
                        UnitQuantity = null,
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 75),
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
                                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 75),
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
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 25),
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
                                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 25),
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
            result = builder.BuildFishingOperation(fishingOperationReport1, fishingOperationModel1, result);

            result.FAReportDocument[^1].SpecifiedFishingActivity[0].RelatedFishingActivity = new FishingActivityType[]
            {
                builder.BuildSubFishingOperation(gearRetrievalModel1)
            };

            result = builder.BuildFishingOperation(fishingOperationReport2, fishingOperationModel2, result);

            result.FAReportDocument[^1].SpecifiedFishingActivity[0].RelatedFishingActivity = new FishingActivityType[]
            {
                builder.BuildSubFishingOperation(gearRetrievalModel2)
            };

            result = builder.BuildFishingOperation(fishingOperationReport3, fishingOperationModel3, result);

            result.FAReportDocument[^1].SpecifiedFishingActivity[0].RelatedFishingActivity = new FishingActivityType[]
            {
                builder.BuildSubFishingOperation(gearShotModel)
            };

            result = builder.BuildArrivalNotification(arrivalNotificationReport, arrivalNotificationModel, result);
            result = builder.BuildArrivalDeclaration(arrivalDeclarationReport, arrivalDeclarationModel, result);
            result = builder.BuildLanding(landingReport, landingModel, result);

            return new List<FLUXFAReportMessageType> { result };
        }

        private static List<FLUXFAReportMessageType> Test32TwoGearDownAndUpInThreeTripsTrip3(DateTime departureTime)
        {
            DateTime gearRetrievalTime = departureTime.AddHours(2);
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
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 150),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed11 }
                    }
                },
                FishingGear = FaTripConstants.FishingGearDeployed11
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
                builder.BuildSubFishingOperation(gearRetrievalModel)
            };

            result = builder.BuildArrivalNotification(arrivalNotificationReport, arrivalNotificationModel, result);
            result = builder.BuildArrivalDeclaration(arrivalDeclarationReport, arrivalDeclarationModel, result);
            result = builder.BuildLanding(landingReport, landingModel, result);

            return new List<FLUXFAReportMessageType> { result };
        }

        /// <summary>
        /// 1. Тръгване
        /// 2. Пускане на уред 1 (статичен)
        /// 3. Вдигане на уред 1 (50 кг калкан)
        /// 4. Пускане на уред 1 (статичен)
        /// 5. Вдигане на уред 1 (100 кг калкан)
        /// 6. Пускане и вдигане на уред 1 (25 кг калкан)
        /// 7. (Уведомление) и връщане
        /// 8. Разтоварване(175 кг калкан)
        /// </summary>
        /// <returns></returns>
        public static List<FLUXFAReportMessageType> Test33GearDownAndUpSeparateTwiceInSameTripAndOnceTogether()
        {
            DateTime departureTime = DateTime.Now;
            DateTime gearShotTime1 = departureTime.AddHours(2);
            DateTime gearRetrievalTime1 = gearShotTime1.AddHours(1);
            DateTime gearShotTime2 = gearRetrievalTime1.AddMinutes(30);
            DateTime gearRetrievalTime2 = gearShotTime2.AddHours(1);
            DateTime gearShotTime3 = gearRetrievalTime2.AddMinutes(30);
            DateTime gearRetrievalTime3 = gearShotTime3.AddHours(2);
            DateTime arrivalNotificationTime = gearRetrievalTime3.AddHours(2);
            DateTime arrivalDeclarationTime = gearRetrievalTime3.AddHours(2).AddMinutes(10);
            DateTime landingStartTime = arrivalDeclarationTime.AddMinutes(30);
            DateTime landingEndTime = arrivalDeclarationTime.AddMinutes(32);

            FaTripTestBuilder builder = new();

            FaReportModel departureReport = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport1 = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport2 = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport3 = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport4 = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
            FaReportModel fishingOperationReport5 = new(FluxPurposes.Original, null, null, null, FaTripConstants.Vessel1, null);
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

            FaFishingOperationModel fishingOperationModel1 = new()
            {
                VesselRelatedActivityCode = "SET",
                OperationsQuantity = 1,
                SpeciesTargetCode = "TUR",
                FishingGear = FaTripConstants.FishingGearDeployed11
            };

            FaSubFishingOperationModel gearShotModel1 = new()
            {
                TypeCode = "GEAR_SHOT",
                Occurrence = gearShotTime1,
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

            FaFishingOperationModel fishingOperationModel2 = new()
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
                        UnitQuantity = new QuantityType{ unitCode = "C62", Value = 25 },
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 50),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed11 }
                    }
                },
                FishingGear = FaTripConstants.FishingGearDeployed11
            };

            FaSubFishingOperationModel gearRetrievalModel1 = new()
            {
                TypeCode = "GEAR_RETRIEVAL",
                Occurrence = gearRetrievalTime1,
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

            FaFishingOperationModel fishingOperationModel3 = new()
            {
                VesselRelatedActivityCode = "SET",
                OperationsQuantity = 1,
                SpeciesTargetCode = "TUR",
                FishingGear = FaTripConstants.FishingGearDeployed11
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

            FaFishingOperationModel fishingOperationModel4 = new()
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
                FishingGear = FaTripConstants.FishingGearDeployed11
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

            FaFishingOperationModel fishingOperationModel5 = new()
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
                        UnitQuantity = new QuantityType{ unitCode = "C62", Value = 5 },
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 25),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed11 }
                    }
                },
                FishingGear = FaTripConstants.FishingGearDeployed11,
                StartTime = gearShotTime3,
                EndTime = gearRetrievalTime3
            };

            FaSubFishingOperationModel gearShotModel3 = new()
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

            FaSubFishingOperationModel gearRetrievalModel3 = new()
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
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "TUR"),
                        UnitQuantity = new QuantityType{ unitCode = "C62", Value = 25 },
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 50),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed11 }
                    },
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "UNLOADED"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "TUR"),
                        UnitQuantity = new QuantityType{ unitCode = "C62", Value = 50 },
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 100),
                        SpecifiedFLUXLocation = new FLUXLocationType[]{ FaTripConstants.LocationArea, FaTripConstants.LocationPosition1, FaTripConstants.LocationStatRectangle1 },
                        SpecifiedSizeDistribution = new SizeDistributionType{ ClassCode = new CodeType[]{ CodeType.CreateCode("FISH_SIZE_CLASS", "LSC") } },
                        UsedFishingGear = new FishingGearType[]{ FaTripConstants.FishingGearDeployed11 }
                    },
                    new FACatchType
                    {
                        TypeCode = CodeType.CreateCode("FA_CATCH_TYPE", "UNLOADED"),
                        SpeciesCode = CodeType.CreateCode("FAO_SPECIES", "TUR"),
                        UnitQuantity = new QuantityType{ unitCode = "C62", Value = 5 },
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 25),
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
                        UnitQuantity = new QuantityType{ unitCode = "C62", Value = 80 },
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 175),
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
                                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, 175),
                                        PackagingUnitQuantity = new QuantityType{ unitCode = "C62", Value = 80 },
                                        PackagingTypeCode = CodeType.CreateCode("FISH_PACKAGING", "BOX")
                                    }
                                }
                            }
                        }
                    }
                }
            };

            FLUXFAReportMessageType result = builder.BuildDeparture(departureReport, departureModel);

            // first FISHING_OPERATION
            result = builder.BuildFishingOperation(fishingOperationReport1, fishingOperationModel1, result);

            result.FAReportDocument[^1].SpecifiedFishingActivity[0].RelatedFishingActivity = new FishingActivityType[]
            {
                builder.BuildSubFishingOperation(gearShotModel1)
            };

            Guid gearShotGuid1 = Guid.Parse(result.FAReportDocument[^1].RelatedFLUXReportDocument.ID[0].Value);

            // second FISHING_OPERATION
            result = builder.BuildFishingOperation(fishingOperationReport2, fishingOperationModel2, result);

            result.FAReportDocument[^1].SpecifiedFishingActivity[0].RelatedFishingActivity = new FishingActivityType[]
            {
                builder.BuildSubFishingOperation(gearRetrievalModel1)
            };

            result.FAReportDocument[^1].RelatedReportID = new IDType[] { IDType.CreateFromGuid(gearShotGuid1) };

            // third FISHING_OPERATION
            result = builder.BuildFishingOperation(fishingOperationReport3, fishingOperationModel3, result);

            result.FAReportDocument[^1].SpecifiedFishingActivity[0].RelatedFishingActivity = new FishingActivityType[]
            {
                builder.BuildSubFishingOperation(gearShotModel2)
            };

            Guid gearShotGuid2 = Guid.Parse(result.FAReportDocument[^1].RelatedFLUXReportDocument.ID[0].Value);

            // fourth FISHING_OPERATION
            result = builder.BuildFishingOperation(fishingOperationReport4, fishingOperationModel4, result);

            result.FAReportDocument[^1].SpecifiedFishingActivity[0].RelatedFishingActivity = new FishingActivityType[]
            {
                builder.BuildSubFishingOperation(gearRetrievalModel2)
            };

            result.FAReportDocument[^1].RelatedReportID = new IDType[] { IDType.CreateFromGuid(gearShotGuid2) };

            // fifth FISHING_OPERATION
            result = builder.BuildFishingOperation(fishingOperationReport5, fishingOperationModel5, result);

            result.FAReportDocument[^1].SpecifiedFishingActivity[0].RelatedFishingActivity = new FishingActivityType[]
            {
                builder.BuildSubFishingOperation(gearShotModel3),
                builder.BuildSubFishingOperation(gearRetrievalModel3)
            };

            result = builder.BuildArrivalNotification(arrivalNotificationReport, arrivalNotificationModel, result);
            result = builder.BuildArrivalDeclaration(arrivalDeclarationReport, arrivalDeclarationModel, result);
            result = builder.BuildLanding(landingReport, landingModel, result);

            return new List<FLUXFAReportMessageType> { result };
        }
    }
}
