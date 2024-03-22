using System;
using System.Linq;
using IARA.Flux.Models;
using IARA.Flux.Tests.FATests.Models;
using IARA.FluxModels.Enums;
using IARA.Infrastructure.FluxIntegrations.Utils;

namespace IARA.Flux.Tests.FATests
{
    public class FaTripBuilder
    {
        private readonly string tripIdentifier;

        private Guid[] relatedReportIds;
        private VesselTransportMeansType vesselTransportMeans;

        public FaTripBuilder()
        {
            this.tripIdentifier = FluxIdentifierGenerator.GenerateTripIdentifier();
        }

        public FLUXFAReportMessageType GenerateReport(Guid? referencedId = null, string purpose = null)
        {
            FLUXFAReportMessageType result = new()
            {
                FLUXReportDocument = new FLUXReportDocumentType
                {
                    ID = new IDType[] { IDType.CreateFromGuid(Guid.NewGuid()) },
                    CreationDateTime = DateTime.Now,
                    PurposeCode = CodeType.CreatePurpose(ReportPurposeCodes.Original),
                    OwnerFLUXParty = FaTripConstants.BgrOwnerParty
                }
            };

            if (referencedId.HasValue)
            {
                result.FLUXReportDocument.ReferencedID = IDType.CreateFromGuid(referencedId.Value);
            }

            if (!string.IsNullOrEmpty(purpose))
            {
                result.FLUXReportDocument.Purpose = TextType.CreateText(purpose);
            }

            return result;
        }

        public FAReportDocumentType GenerateNotification(FaReportModel model)
        {
            relatedReportIds = model.RelatedReportIds;
            vesselTransportMeans = model.VesselTransportMeans;
            return GenerateReportDocument("NOTIFICATION", ((int)model.Purpose).ToString(), model.ReferencedId, model.PurposeText, model.FmcMarkerCode);
        }

        public FAReportDocumentType GenerateDeclaration(FaReportModel model)
        {
            relatedReportIds = model.RelatedReportIds;
            vesselTransportMeans = model.VesselTransportMeans;
            return GenerateReportDocument("DECLARATION", ((int)model.Purpose).ToString(), model.ReferencedId, model.PurposeText, model.FmcMarkerCode);
        }

        public FishingActivityType GenerateDeparture(FaDepartureModel model)
        {
            FishingActivityType result = new()
            {
                TypeCode = CodeType.CreateCode("FLUX_FA_TYPE", "DEPARTURE"),
                OccurrenceDateTime = model.Occurrence,
                SpecifiedFishingTrip = GenerateFishingTripIdentifier(),
                RelatedFLUXLocation = model.Locations,
                SpecifiedFACatch = model.Catches,
                SpecifiedFishingGear = model.FishingGears
            };

            if (!string.IsNullOrEmpty(model.ReasonCode))
            {
                result.ReasonCode = CodeType.CreateCode("FA_REASON_DEPARTURE", model.ReasonCode);
            }

            if (!string.IsNullOrEmpty(model.FisheryTypeCode))
            {
                result.FisheryTypeCode = CodeType.CreateCode("FA_FISHERY", model.FisheryTypeCode);
            }

            if (!string.IsNullOrEmpty(model.SpeciesTargetCode))
            {
                result.SpeciesTargetCode = CodeType.CreateCode("FAO_SPECIES", model.SpeciesTargetCode);
            }
            else if (!string.IsNullOrEmpty(model.SpeciesTargetGroupCode))
            {
                result.SpeciesTargetCode = CodeType.CreateCode("TARGET_SPECIES_GROUP", model.SpeciesTargetGroupCode);
            }

            return result;
        }

        public FishingActivityType GenerateAreaEntry(FaAreaEntryModel model)
        {
            FishingActivityType result = new()
            {
                TypeCode = CodeType.CreateCode("FLUX_FA_TYPE", "AREA_ENTRY"),
                OccurrenceDateTime = model.Occurrence,
                SpecifiedFishingTrip = GenerateFishingTripIdentifier(),
                RelatedFLUXLocation = model.Locations,
                SpecifiedFACatch = model.Catches
            };

            if (!string.IsNullOrEmpty(model.ReasonCode))
            {
                result.ReasonCode = CodeType.CreateCode("FA_REASON_ENTRY", model.ReasonCode);
            }

            if (!string.IsNullOrEmpty(model.FisheryTypeCode))
            {
                result.FisheryTypeCode = CodeType.CreateCode("FA_FISHERY", model.FisheryTypeCode);
            }

            if (!string.IsNullOrEmpty(model.SpeciesTargetGroupCode))
            {
                result.SpeciesTargetCode = CodeType.CreateCode("TARGET_SPECIES_GROUP", model.SpeciesTargetGroupCode);
            }

            return result;
        }

        public FishingActivityType GenerateAreaExit(FaAreaExitModel model)
        {
            FishingActivityType result = new()
            {
                TypeCode = CodeType.CreateCode("FLUX_FA_TYPE", "AREA_EXIT"),
                OccurrenceDateTime = model.Occurrence,
                SpecifiedFishingTrip = GenerateFishingTripIdentifier(),
                RelatedFLUXLocation = model.Locations,
                SpecifiedFACatch = model.Catches
            };

            if (!string.IsNullOrEmpty(model.FisheryTypeCode))
            {
                result.FisheryTypeCode = CodeType.CreateCode("FA_FISHERY", model.FisheryTypeCode);
            }

            if (!string.IsNullOrEmpty(model.SpeciesTargetCode))
            {
                result.SpeciesTargetCode = CodeType.CreateCode("FAO_SPECIES", model.SpeciesTargetCode);
            }

            return result;
        }

        public FishingActivityType GenerateFishingOperation(FaFishingOperationModel model)
        {
            FishingActivityType result = new()
            {
                TypeCode = CodeType.CreateCode("FLUX_FA_TYPE", "FISHING_OPERATION"),
                OccurrenceDateTime = model.Occurrence,
                SpecifiedFishingTrip = GenerateFishingTripIdentifier(),
                RelatedFLUXLocation = model.Locations,
                SpecifiedFACatch = model.Catches,
                RelatedVesselTransportMeans = model.RelatedVesselTransportMeans,
                SpecifiedFishingGear = model.FishingGear != null ? new FishingGearType[] { model.FishingGear } : null,
                SpecifiedGearProblem = model.GearProblems,
                SpecifiedFLUXCharacteristic = model.Characteristics
            };

            if (!string.IsNullOrEmpty(model.VesselRelatedActivityCode))
            {
                result.VesselRelatedActivityCode = CodeType.CreateCode("VESSEL_ACTIVITY", model.VesselRelatedActivityCode);
            }

            if (model.OperationsQuantity.HasValue)
            {
                result.OperationsQuantity = new QuantityType
                {
                    Value = model.OperationsQuantity.Value,
                    unitCode = "C62"
                };
            }

            if (!string.IsNullOrEmpty(model.FisheryTypeCode))
            {
                result.FisheryTypeCode = CodeType.CreateCode("FA_FISHERY", model.FisheryTypeCode);
            }

            if (!string.IsNullOrEmpty(model.SpeciesTargetCode))
            {
                result.SpeciesTargetCode = CodeType.CreateCode("FAO_SPECIES", model.SpeciesTargetCode);
            }

            if (model.StartTime.HasValue && model.EndTime.HasValue)
            {
                result.SpecifiedDelimitedPeriod = new DelimitedPeriodType[]
                {
                    new DelimitedPeriodType
                    {
                        StartDateTime = model.StartTime.Value,
                        EndDateTime = model.EndTime.Value
                    }
                };
            }

            return result;
        }

        public FishingActivityType GenerateSubFishingOperation(FaSubFishingOperationModel model)
        {
            FishingActivityType result = new()
            {
                TypeCode = CodeType.CreateCode("FLUX_FA_TYPE", model.TypeCode),
                OccurrenceDateTime = model.Occurrence,
                RelatedFLUXLocation = model.Location != null ? new FLUXLocationType[] { model.Location } : null,
                SpecifiedFishingGear = model.FishingGear != null ? new FishingGearType[] { model.FishingGear } : null,
                SpecifiedGearProblem = model.GearProblems,
                SpecifiedFLUXCharacteristic = model.Characteristics
            };

            if (model.StartTime.HasValue && model.EndTime.HasValue)
            {
                result.SpecifiedDelimitedPeriod = new DelimitedPeriodType[]
                {
                    new DelimitedPeriodType
                    {
                        StartDateTime = model.StartTime.Value,
                        EndDateTime = model.EndTime.Value
                    }
                };
            }

            return result;
        }

        public FishingActivityType GenerateJointFishingOperation(FaJointFishingOperationModel model)
        {
            FishingActivityType result = new()
            {
                TypeCode = CodeType.CreateCode("FLUX_FA_TYPE", "JOINT_FISHING_OPERATION"),
                OccurrenceDateTime = model.Occurrence,
                SpecifiedFishingTrip = GenerateFishingTripIdentifier("JFO"),
                RelatedFLUXLocation = model.Locations,
                SpecifiedFACatch = model.Catches,
                RelatedVesselTransportMeans = model.RelatedVesselTransportMeans,
                SpecifiedFishingGear = model.FishingGear != null ? new FishingGearType[] { model.FishingGear } : null,
                SpecifiedGearProblem = model.GearProblems,
                SpecifiedFLUXCharacteristic = model.Characteristics
            };

            if (!string.IsNullOrEmpty(model.VesselRelatedActivityCode))
            {
                result.VesselRelatedActivityCode = CodeType.CreateCode("VESSEL_ACTIVITY", model.VesselRelatedActivityCode);
            }

            if (!string.IsNullOrEmpty(model.FisheryTypeCode))
            {
                result.FisheryTypeCode = CodeType.CreateCode("FA_FISHERY", model.FisheryTypeCode);
            }

            if (!string.IsNullOrEmpty(model.SpeciesTargetCode))
            {
                result.SpeciesTargetCode = CodeType.CreateCode("FAO_SPECIES", model.SpeciesTargetCode);
            }

            if (model.StartTime.HasValue && model.EndTime.HasValue)
            {
                result.SpecifiedDelimitedPeriod = new DelimitedPeriodType[]
                {
                    new DelimitedPeriodType
                    {
                        StartDateTime = model.StartTime.Value,
                        EndDateTime = model.EndTime.Value
                    }
                };
            }

            return result;
        }

        public FishingActivityType GenerateSubJointFishingOperation(FaSubJointFishingOperationModel model)
        {
            FishingActivityType result = new()
            {
                TypeCode = CodeType.CreateCode("FLUX_FA_TYPE", "RELOCATION"),
                RelatedVesselTransportMeans = model.RelatedVesselTransportMeans != null ? new VesselTransportMeansType[] { model.RelatedVesselTransportMeans } : null,
                SpecifiedFACatch = model.Catches,
                SpecifiedFLUXCharacteristic = model.Characteristics
            };

            return result;
        }

        public FishingActivityType GenerateDiscard(FaDiscardModel model)
        {
            FishingActivityType result = new()
            {
                TypeCode = CodeType.CreateCode("FLUX_FA_TYPE", "DISCARD"),
                OccurrenceDateTime = model.Occurrence,
                SpecifiedFishingTrip = GenerateFishingTripIdentifier(),
                RelatedFLUXLocation = model.Locations,
                SpecifiedFACatch = model.Catches,
                SpecifiedFLUXCharacteristic = model.Characteristics
            };

            if (!string.IsNullOrEmpty(model.ReasonCode))
            {
                result.ReasonCode = CodeType.CreateCode("FA_REASON_DISCARD", model.ReasonCode);
            }

            return result;
        }

        public FishingActivityType GenerateRelocationNotification(FaRelocationNotificationModel model)
        {
            FishingActivityType result = new()
            {
                TypeCode = CodeType.CreateCode("FLUX_FA_TYPE", "RELOCATION"),
                OccurrenceDateTime = model.Occurrence,
                SpecifiedFishingTrip = GenerateFishingTripIdentifier(),
                RelatedFLUXLocation = model.Locations,
                SpecifiedFACatch = model.Catches,
                RelatedVesselTransportMeans = model.RelatedVesselTransportMeans != null ? new VesselTransportMeansType[] { model.RelatedVesselTransportMeans } : null,
                DestinationVesselStorageCharacteristic = model.DestinationVesselStorageCharacteristics,
                SpecifiedFLUXCharacteristic = model.Characteristics,
                SpecifiedFLAPDocument = model.FLAPDocuments
            };

            return result;
        }

        public FishingActivityType GenerateRelocationDeclaration(FaRelocationModel model)
        {
            FishingActivityType result = new()
            {
                TypeCode = CodeType.CreateCode("FLUX_FA_TYPE", "RELOCATION"),
                OccurrenceDateTime = model.Occurrence,
                SpecifiedFishingTrip = GenerateFishingTripIdentifier(),
                RelatedFLUXLocation = model.Locations,
                SpecifiedFACatch = model.Catches,
                RelatedVesselTransportMeans = model.RelatedVesselTransportMeans != null ? new VesselTransportMeansType[] { model.RelatedVesselTransportMeans } : null,
                SourceVesselStorageCharacteristic = model.SourceVesselStorageCharacteristics,
                DestinationVesselStorageCharacteristic = model.DestinationVesselStorageCharacteristics,
                SpecifiedFLAPDocument = model.FLAPDocuments
            };

            return result;
        }

        public FishingActivityType GenerateTranshipmentNotification(FaTranshipmentNotificationModel model)
        {
            FishingActivityType result = new()
            {
                TypeCode = CodeType.CreateCode("FLUX_FA_TYPE", "TRANSHIPMENT"),
                OccurrenceDateTime = model.Occurrence,
                SpecifiedFishingTrip = GenerateFishingTripIdentifier(),
                RelatedFLUXLocation = model.Locations,
                SpecifiedFACatch = model.Catches,
                RelatedVesselTransportMeans = model.RelatedVesselTransportMeans != null ? new VesselTransportMeansType[] { model.RelatedVesselTransportMeans } : null,
                DestinationVesselStorageCharacteristic = model.DestinationVesselStorageCharacteristics,
                SpecifiedFLUXCharacteristic = model.Characteristics,
                SpecifiedFLAPDocument = model.FLAPDocuments
            };

            return result;
        }

        public FishingActivityType GenerateTranshipmentDeclaration(FaTranshipmentModel model)
        {
            FishingActivityType result = new()
            {
                TypeCode = CodeType.CreateCode("FLUX_FA_TYPE", "TRANSHIPMENT"),
                SpecifiedDelimitedPeriod = new DelimitedPeriodType[]
                {
                    new DelimitedPeriodType
                    {
                        StartDateTime = model.StartTime,
                        EndDateTime = model.EndTime
                    }
                },
                SpecifiedFishingTrip = GenerateFishingTripIdentifier(),
                RelatedFLUXLocation = model.Locations,
                SpecifiedFACatch = model.Catches,
                RelatedVesselTransportMeans = model.RelatedVesselTransportMeans != null ? new VesselTransportMeansType[] { model.RelatedVesselTransportMeans } : null,
                SpecifiedFLAPDocument = model.FLAPDocuments,
                SpecifiedFLUXCharacteristic = model.Characteristics
            };

            return result;
        }

        public FishingActivityType GenerateArrivalNotification(FaArrivalNotificationModel model)
        {
            FishingActivityType result = new()
            {
                TypeCode = CodeType.CreateCode("FLUX_FA_TYPE", "ARRIVAL"),
                OccurrenceDateTime = model.Occurrence,
                ReasonCode = CodeType.CreateCode("FA_REASON_ARRIVAL", model.ReasonCode),
                SpecifiedFishingTrip = GenerateFishingTripIdentifier(startTime: model.TripStartDate, endTime: model.TripEndDate),
                RelatedFLUXLocation = model.Locations,
                SpecifiedFACatch = model.Catches
            };

            return result;
        }

        public FishingActivityType GenerateArrivalDeclaration(FaArrivalDeclarationModel model)
        {
            FishingActivityType result = new()
            {
                TypeCode = CodeType.CreateCode("FLUX_FA_TYPE", "ARRIVAL"),
                OccurrenceDateTime = model.Occurrence,
                SpecifiedFishingTrip = GenerateFishingTripIdentifier(),
                RelatedFLUXLocation = model.Locations,
                SpecifiedFishingGear = model.FishingGears,
                SpecifiedFLUXCharacteristic = model.Characteristics
            };

            if (!string.IsNullOrEmpty(model.ReasonCode))
            {
                result.ReasonCode = CodeType.CreateCode("FA_REASON_ARRIVAL", model.ReasonCode);
            }

            return result;
        }

        public FishingActivityType GenerateLanding(FaLandingModel model)
        {
            FishingActivityType result = new()
            {
                TypeCode = CodeType.CreateCode("FLUX_FA_TYPE", "LANDING"),
                SpecifiedDelimitedPeriod = new DelimitedPeriodType[]
                {
                    new DelimitedPeriodType
                    {
                        StartDateTime = model.DateFrom,
                        EndDateTime = model.DateTo
                    }
                },
                SpecifiedFishingTrip = GenerateFishingTripIdentifier(),
                RelatedFLUXLocation = model.Locations,
                SpecifiedFACatch = model.Catches,
                SpecifiedFLUXCharacteristic = model.Characteristics
            };

            return result;
        }

        private FAReportDocumentType GenerateReportDocument(string type,
                                                            string purposeCode,
                                                            Guid? referencedId = null,
                                                            string purposeText = null,
                                                            string fmcMarkerCode = null)
        {
            FAReportDocumentType result = new()
            {
                TypeCode = CodeType.CreateCode("FLUX_FA_REPORT_TYPE", type),
                AcceptanceDateTime = DateTime.Now,
                RelatedFLUXReportDocument = new FLUXReportDocumentType
                {
                    ID = new IDType[] { IDType.CreateFromGuid(Guid.NewGuid()) },
                    CreationDateTime = DateTime.Now,
                    PurposeCode = CodeType.CreateCode("FLUX_GP_PURPOSE", purposeCode),
                    OwnerFLUXParty = FaTripConstants.BgrOwnerParty
                },
                SpecifiedVesselTransportMeans = vesselTransportMeans
            };

            if (relatedReportIds != null && relatedReportIds.Any())
            {
                result.RelatedReportID = relatedReportIds.Select(IDType.CreateFromGuid).ToArray();
            }

            if (referencedId.HasValue)
            {
                result.RelatedFLUXReportDocument.ReferencedID = IDType.CreateFromGuid(referencedId.Value);
            }

            if (!string.IsNullOrEmpty(purposeText))
            {
                result.RelatedFLUXReportDocument.Purpose = TextType.CreateText(purposeText);
            }

            if (!string.IsNullOrEmpty(fmcMarkerCode))
            {
                result.FMCMarkerCode = CodeType.CreateCode("FLUX_FA_FMC", fmcMarkerCode);
            }

            return result;
        }

        private FishingTripType GenerateFishingTripIdentifier(string type = null, DateTime? startTime = null, DateTime? endTime = null)
        {
            FishingTripType result = new()
            {
                ID = new IDType[] { IDType.CreateID("EU_TRIP_ID", tripIdentifier) }
            };

            if (!string.IsNullOrEmpty(type))
            {
                result.TypeCode = CodeType.CreateCode("FISHING_TRIP_TYPE", type);
            }

            if (startTime.HasValue && endTime.HasValue)
            {
                result.SpecifiedDelimitedPeriod = new DelimitedPeriodType[]
                {
                    new DelimitedPeriodType
                    {
                        StartDateTime = startTime.Value,
                        EndDateTime = endTime.Value
                    }
                };
            }

            return result;
        }
    }
}
