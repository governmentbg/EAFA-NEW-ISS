using System;
using System.Collections.Generic;
using System.Linq;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.FLUXVMSRequests;
using IARA.Flux.Models;
using IARA.FluxModels;
using IARA.FluxModels.Enums;
using IARA.Interfaces.FluxIntegrations.PermitsAndCertificates;

namespace IARA.Infrastructure.FluxIntegrations.Mappings
{
    public class FlapDomainMapper : BaseService, IFlapDomainMapper
    {
        public const string BULGARIA_CODE = "BGR";
        public const string BULGARIA_NAME_EN = "Bulgaria";

        public FlapDomainMapper(IARADbContext dbContext)
            : base(dbContext)
        { }

        public FLUXFLAPRequestMessageType MapRequestToFlux(FluxFlapRequestEditDTO request, Guid referenceId, ReportPurposeCodes purpose)
        {
            FLUXFLAPRequestMessageType result = new()
            {
                FLUXReportDocument = CreateReportDocument(referenceId, purpose),
                FLAPRequestDocument = CreateRequestDocument(request),
                FLAPDocument = CreateFlapDocument(request)
            };

            return result;
        }

        public FluxFlapRequestEditDTO MapFluxToRequest(FLUXFLAPRequestMessageType request)
        {
            FluxFlapRequestEditDTO result = new()
            {
                AgreementTypeCode = request.FLAPRequestDocument.TypeCode.Value,
                CoastalPartyCode = request.FLAPRequestDocument.FADATypeCode.Value,
                RequestPurposeCode = request.FLAPRequestDocument.PurposeCode.Value,
                RequestPurposeText = request.FLAPRequestDocument.Purpose?.Value,
                FishingCategoryCode = request.FLAPRequestDocument.RelatedFishingCategory.TypeCode.Value,
                FishingMethod = string.Join("\n", GetFishingMethodsAsText(request.FLAPRequestDocument.RelatedFishingCategory)),
                FishingArea = string.Join("\n", GetFishingAreasAsText(request.FLAPRequestDocument.RelatedFishingCategory)),
                AuthorizedFishingGearCodes = request.FLAPRequestDocument.RelatedFishingCategory.AuthorizedFishingGear.Select(x => x.TypeCode.Value).ToList(),
                Ship = new FluxFlapRequestShipDTO
                {
                    ShipIdentifierType = request.FLAPDocument.VesselID[0].schemeID,
                    ShipIdentifier = request.FLAPDocument.VesselID[0].Value,
                    ShipName = "N/A"
                },
                JoinedShips = GetJoinedShips(request.FLAPDocument.JoinedVesselID),
                IsFirstApplication = request.FLAPDocument.FirstApplicationIndicator?.Item?.ToString() == "Y",
                Remarks = request.FLAPDocument.Remarks?.Value,
                LocalSeamenCount = (int?)request.FLAPDocument.SpecifiedVesselCrew
                    ?.Where(x => x.TypeCode.Value == nameof(VesselCrewTypes.LOCAL_SEAMEN))
                    ?.Select(x => x.MemberQuantity.Value)
                    ?.First(),
                AcpSeamenCount = (int?)request.FLAPDocument.SpecifiedVesselCrew
                    ?.Where(x => x.TypeCode.Value == nameof(VesselCrewTypes.ACP_SEAMEN))
                    ?.Select(x => x.MemberQuantity.Value)
                    ?.First(),
                AuthorizationStartDate = request.FLAPDocument.ApplicableDelimitedPeriod != null && request.FLAPDocument.ApplicableDelimitedPeriod.Length > 0
                    ? request.FLAPDocument.ApplicableDelimitedPeriod[0].StartDateTime.Item
                    : null,
                AuthorizationEndDate = request.FLAPDocument.ApplicableDelimitedPeriod != null && request.FLAPDocument.ApplicableDelimitedPeriod.Length > 0
                    ? request.FLAPDocument.ApplicableDelimitedPeriod[0].EndDateTime.Item
                    : null,
                TargetedQuotas = GetTargetedQuotas(request.FLAPDocument.SpecifiedTargetedQuota)
            };

            return result;
        }

        // Model to FLUX
        private static FLUXReportDocumentType CreateReportDocument(Guid referenceId, ReportPurposeCodes purpose)
        {
            FLUXReportDocumentType document = new()
            {
                ID = new IDType[] { IDType.CreateFromGuid(referenceId) },
                PurposeCode = CodeType.CreatePurpose(purpose),
                CreationDateTime = DateTime.Now,
                OwnerFLUXParty = new FLUXPartyType
                {
                    ID = new IDType[] { IDType.CreateParty(BULGARIA_CODE) },
                    Name = new TextType[] { TextType.CreateText(BULGARIA_NAME_EN) }
                }
            };

            return document;
        }

        private static FLAPRequestDocumentType CreateRequestDocument(FluxFlapRequestEditDTO request)
        {
            FLAPRequestDocumentType document = new()
            {
                TypeCode = CodeType.CreateCode(ListIDTypes.AGREEMENT_TYPE, request.AgreementTypeCode),
                FADATypeCode = CodeType.CreateCode(ListIDTypes.FLAP_COASTAL_PARTY, request.CoastalPartyCode),
                PurposeCode = CodeType.CreateCode(ListIDTypes.FLAP_REQUEST_PURPOSE, request.RequestPurposeCode),
                RelatedFishingCategory = CreateFishingCategory(request)
            };

            if (!string.IsNullOrEmpty(request.RequestPurposeText))
            {
                document.Purpose = TextType.CreateText(request.RequestPurposeText);
            }

            return document;
        }

        private static FishingCategoryType CreateFishingCategory(FluxFlapRequestEditDTO request)
        {
            FishingCategoryType result = new()
            {
                TypeCode = CodeType.CreateCode(ListIDTypes.FAR_FISH_CATEGORY, request.FishingCategoryCode),
                FishingMethod = new TextType[] { TextType.CreateText(request.FishingMethod) },
                FishingArea = new TextType[] { TextType.CreateText(request.FishingArea) },
                AuthorizedFishingGear = CreateAuthorizedFishingGear(request)
            };

            return result;
        }

        private static FishingGearType[] CreateAuthorizedFishingGear(FluxFlapRequestEditDTO request)
        {
            FishingGearType[] result = (from gear in request.AuthorizedFishingGearCodes
                                        select new FishingGearType
                                        {
                                            TypeCode = CodeType.CreateCode(ListIDTypes.GEAR_TYPE, gear)
                                        }).ToArray();

            return result;
        }

        private FLAPDocumentType CreateFlapDocument(FluxFlapRequestEditDTO request)
        {
            List<int> joinedShipIds = request.JoinedShips != null
                ? request.JoinedShips.Select(x => x.ShipId.Value).ToList()
                : new List<int>();

            List<int> shipIds = joinedShipIds.Concat(new List<int> { request.Ship.ShipId.Value }).ToList();

            Dictionary<int, string> cfrs = GetShipCfrs(shipIds);

            FLAPDocumentType document = new()
            {
                VesselID = new IDType[] { IDType.CreateCFR(cfrs[request.Ship.ShipId.Value]) },
                JoinedVesselID = CreatedJoinedVesselIds(joinedShipIds, cfrs),
                FirstApplicationIndicator = new IndicatorType { Item = request.IsFirstApplication.Value ? "Y" : "N" },
                TypeCode = GetVesselTypeCode(request.Ship.ShipId.Value),
                SpecifiedVesselCrew = CreateVesselCrew(request),
                ApplicableDelimitedPeriod = CreateDelimitedPeriod(request.AuthorizationStartDate.Value, request.AuthorizationEndDate.Value),
                SpecifiedTargetedQuota = CreatedTargetedQuota(request.TargetedQuotas)
            };

            if (!string.IsNullOrEmpty(request.Remarks))
            {
                document.Remarks = TextType.CreateText(request.Remarks);
            }

            return document;
        }

        private static IDType[] CreatedJoinedVesselIds(List<int> shipIds, Dictionary<int, string> cfrs)
        {
            if (shipIds != null && shipIds.Count > 0)
            {
                IDType[] result = shipIds.Select(x => IDType.CreateCFR(cfrs[x])).ToArray();
                return result;
            }

            return null;
        }

        private CodeType GetVesselTypeCode(int shipId)
        {
            int? vesselTypeId = (from ship in Db.ShipsRegister
                                 where ship.Id == shipId
                                 select ship.VesselTypeId).First();

            if (vesselTypeId.HasValue)
            {
                string code = (from type in Db.NvesselTypes
                               where type.Id == vesselTypeId.Value
                               select type.Code).First();

                return CodeType.CreateVesselType(code);
            }

            // FISHING VESSELS NOT SPECIFIED
            return CodeType.CreateVesselType("FX");
        }

        private static VesselCrewType[] CreateVesselCrew(FluxFlapRequestEditDTO request)
        {
            List<VesselCrewType> result = new();

            if (request.LocalSeamenCount.HasValue)
            {
                result.Add(new VesselCrewType
                {
                    TypeCode = CodeType.CreateCode(ListIDTypes.VESSEL_CREW_TYPE, nameof(VesselCrewTypes.LOCAL_SEAMEN)),
                    MemberQuantity = new QuantityType { Value = request.LocalSeamenCount.Value }
                });
            }

            if (request.AcpSeamenCount.HasValue)
            {
                result.Add(new VesselCrewType
                {
                    TypeCode = CodeType.CreateCode(ListIDTypes.VESSEL_CREW_TYPE, nameof(VesselCrewTypes.ACP_SEAMEN)),
                    MemberQuantity = new QuantityType { Value = request.AcpSeamenCount.Value }
                });
            }

            return result.Count > 0 ? result.ToArray() : null;
        }

        private static DelimitedPeriodType[] CreateDelimitedPeriod(DateTime dateFrom, DateTime dateTo)
        {
            DelimitedPeriodType[] result = new DelimitedPeriodType[]
            {
                new DelimitedPeriodType
                {
                    StartDateTime = DateTimeType.BuildDateTime(dateFrom),
                    EndDateTime = DateTimeType.BuildDateTime(dateTo)
                }
            };

            return result;
        }

        private static TargetedQuotaType[] CreatedTargetedQuota(List<FluxFlapRequestTargetedQuotaDTO> quotas)
        {
            TargetedQuotaType[] result = (from quota in quotas
                                          select new TargetedQuotaType
                                          {
                                              TypeCode = CodeType.CreateCode(ListIDTypes.FLAP_QUOTA_TYPE, quota.FlapQuotaTypeCode),
                                              ObjectCode = CodeType.CreateCode(ListIDTypes.FAO_SPECIES, quota.SpeciesCode),
                                              WeightMeasure = new MeasureType[] { MeasureType.CreateMeasure(FluxUnits.TNE, quota.Tonnage.Value) }
                                          }).ToArray();

            return result;
        }

        private Dictionary<int, string> GetShipCfrs(List<int> shipIds)
        {
            Dictionary<int, string> cfrs = (from ship in Db.ShipsRegister
                                            where shipIds.Contains(ship.Id)
                                            select new
                                            {
                                                ShipId = ship.Id,
                                                ShipCfr = ship.Cfr
                                            }).ToDictionary(x => x.ShipId, y => y.ShipCfr);

            return cfrs;
        }

        // FLUX to model
        private static List<string> GetFishingMethodsAsText(FishingCategoryType fishingCategory)
        {
            List<string> result = new();

            if (fishingCategory != null)
            {
                if (fishingCategory.FishingMethodCode != null)
                {
                    foreach (CodeType method in fishingCategory.FishingMethodCode)
                    {
                        result.Add(method.Value);
                    }
                }

                if (fishingCategory.FishingMethod != null)
                {
                    foreach (TextType method in fishingCategory.FishingMethod)
                    {
                        result.Add(method.Value);
                    }
                }
            }

            return result;
        }

        private static List<string> GetFishingAreasAsText(FishingCategoryType fishingCategory)
        {
            List<string> result = new();

            if (fishingCategory != null)
            {
                if (fishingCategory.FishingAreaCode != null)
                {
                    foreach (CodeType area in fishingCategory.FishingAreaCode)
                    {
                        result.Add(area.Value);
                    }
                }

                if (fishingCategory.FishingArea != null)
                {
                    foreach (TextType area in fishingCategory.FishingArea)
                    {
                        result.Add(area.Value);
                    }
                }
            }
            return result;
        }

        private static List<FluxFlapRequestShipDTO> GetJoinedShips(IDType[] ships)
        {
            if (ships == null || ships.Length == 0)
            {
                return null;
            }

            List<FluxFlapRequestShipDTO> result = (from ship in ships
                                                   select new FluxFlapRequestShipDTO
                                                   {
                                                       ShipIdentifierType = ship.schemeID,
                                                       ShipIdentifier = ship.Value,
                                                       ShipName = "N/A"
                                                   }).ToList();

            return result;
        }

        private static List<FluxFlapRequestTargetedQuotaDTO> GetTargetedQuotas(TargetedQuotaType[] quotas)
        {
            if (quotas == null || quotas.Length == 0)
            {
                return null;
            }

            List<FluxFlapRequestTargetedQuotaDTO> result = (from quota in quotas
                                                            select new FluxFlapRequestTargetedQuotaDTO
                                                            {
                                                                FlapQuotaTypeCode = quota.TypeCode.Value,
                                                                SpeciesCode = quota.ObjectCode.Value,
                                                                Tonnage = quota.WeightMeasure != null && quota.WeightMeasure.Length != 0
                                                                    ? quota.WeightMeasure.Where(x => x.unitCode == nameof(FluxUnits.TNE)).Select(x => x.Value).First()
                                                                    : 0
                                                            }).ToList();

            return result;
        }
    }
}
