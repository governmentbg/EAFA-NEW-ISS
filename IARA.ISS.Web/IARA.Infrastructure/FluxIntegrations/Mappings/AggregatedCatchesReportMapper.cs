using System;
using IARA.DataAccess;
using IARA.Flux.Models;
using System.Linq;
using IARA.FluxModels.Enums;
using System.Globalization;
using IARA.FluxModels;
using System.Collections.Generic;
using IARA.Infrastructure.Services;
using IARA.Common.Enums;
using IARA.Interfaces.FluxIntegrations.AggregatedCatchReports;

namespace IARA.Infrastructure.FluxIntegrations.Mappings
{
    public class AggregatedCatchesReportMapper : BaseService, IAggregatedCatchesReportMapper
    {
        private SystemPropertiesService systemPropertiesService;

        public AggregatedCatchesReportMapper(IARADbContext dbContext, SystemPropertiesService systemPropertiesService)
            : base(dbContext)
        {
            this.systemPropertiesService = systemPropertiesService;
        }

        public FLUXACDRMessageType GetAggragatedCatchesForPeriod(DateTime fromDate, DateTime toDate, Guid? referenceGuid = null)
        {
            var excludeFishCodes = new string[] { "FGI", "AYS", "EIK", "CRG", "PAM", "CRD", "AAS", "NER", "AAJ", "ELA", "OYG", "COH", "MUR", "" };
            var excludeFleetCodes = new string[] { "2", "4", "7", "5" };

            var result = (from x in (from page in Db.ShipLogBookPages
                                     join declaration in Db.OriginDeclarations on page.Id equals declaration.LogBookPageId
                                     join fishDeclaration in Db.OriginDeclarationFish on declaration.Id equals fishDeclaration.OriginDeclarationId
                                     join fishType in Db.Nfishes on fishDeclaration.FishId equals fishType.Id
                                     join fishGroup in Db.NfishGroups on fishType.FishGroupId equals fishGroup.Id into left
                                     from fishGroup in left.DefaultIfEmpty()
                                     join logBookPermitLicence in Db.LogBookPermitLicenses on page.LogBookPermitLicenceId equals logBookPermitLicence.Id
                                     join permitLicence in Db.CommercialFishingPermitLicensesRegisters on logBookPermitLicence.PermitLicenseRegisterId equals permitLicence.Id
                                     join ship in Db.ShipsRegister on permitLicence.ShipId equals ship.Id
                                     join fleet in Db.NfleetTypes on ship.FleetTypeId equals fleet.Id
                                     where page.Status == LogBookPageStatusesEnum.Submitted.ToString()
                                     && !excludeFleetCodes.Contains(fleet.Code)
                                     && !excludeFishCodes.Contains(fishType.Code)
                                     && (fishGroup == null || fishGroup.Code != "Сладководни")
                                     && page.PageFillDate >= fromDate && page.FishTripEndDateTime <= toDate
                                     select new
                                     {
                                         fishDeclaration.Quantity,
                                         fishType.Name,
                                         fishType.Code
                                     })
                          group x by new { x.Code, x.Name } into grouped
                          select new
                          {
                              grouped.Key.Code,
                              grouped.Key.Name,
                              Quantity = grouped.Sum(x => x.Quantity)
                          }).ToList();

            var faoSpeciesDictionary = Db.MdrFaoSpecies.Select(x => new
            {
                x.Code,
                x.ScientificName
            }).ToDictionary(x => x.Code, x => x.ScientificName);

            //Filter Only FAO SPECIES - MAY BE TEMPORARY
            result = result.Where(x => faoSpeciesDictionary.ContainsKey(x.Code)).ToList();

            FLUXACDRMessageType message = new FLUXACDRMessageType
            {
                FLUXReportDocument = new FLUXReportDocumentType
                {
                    CreationDateTime = DateTime.Now,
                    ID = new IDType[] { IDType.CreateFromGuid(Guid.NewGuid()) },
                    Purpose = $"{fromDate.ToString("MMMM", new CultureInfo("en-US"))} {fromDate.Year} ACRD Report",
                    OwnerFLUXParty = new FLUXPartyType
                    {
                        ID = new IDType[] { new IDType { Value = systemPropertiesService.SystemProperties.ACDRUserID } },
                        Name = TextType.CreateMultiText(systemPropertiesService.SystemProperties.ACDRUserName)
                    }
                },
                AggregatedCatchReportDocument = new AggregatedCatchReportDocumentType
                {
                    EffectiveDelimitedPeriod = new DelimitedPeriodType
                    {
                        StartDateTime = fromDate,
                        EndDateTime = toDate,
                    },
                    OwnerFLUXParty = new FLUXPartyType
                    {
                        ID = new IDType[]
                        {
                            new IDType
                            {
                                schemeID = ListIDTypes.FLUX_GP_PARTY,
                                Value = CountryCodes.BGR.ToString()
                            }
                        },
                        Name = TextType.CreateMultiText("Bulgaria")
                    }
                }
            };

            if (referenceGuid != null && referenceGuid.HasValue)
            {
                message.FLUXReportDocument.ReferencedID = IDType.CreateFromGuid(referenceGuid.Value);
                message.FLUXReportDocument.PurposeCode = CodeType.CreatePurpose(ReportPurposeCodes.Replace);
            }
            else
            {
                message.FLUXReportDocument.PurposeCode = CodeType.CreatePurpose(ReportPurposeCodes.Original);
            }

            FLUXACDRReportType report;

            if (result.Any())
            {
                message.FLUXACDRReport = new List<FLUXACDRReportType>();

                var quotaSpecies = (from x in Db.MdrQuotaObjects
                                    select new
                                    {
                                        x.Code,
                                        x.EnName
                                    }).ToDictionary(x => x.Code, x => x.EnName);


                foreach (var specie in result.Where(x => quotaSpecies.ContainsKey(x.Code)))
                {
                    report = new FLUXACDRReportType
                    {
                        RegionalSpeciesCode = CodeType.CreateCode(CodeTypes.QUOTA_OBJECT, specie.Code),
                        RegionalAreaCode = CodeType.CreateCode(CodeTypes.QUOTA_LOCATION, "F3742C"),
                        TypeCode = CodeType.CreateCode(CodeTypes.ACDR_REPORT_TYPE, AcdReportTypes.ACDR_REGIONAL),
                    };

                    report.SpecifiedACDRReportedArea = new List<ACDRReportedAreaType>();

                    report.SpecifiedACDRReportedArea.Add(new ACDRReportedAreaType
                    {
                        CatchStatusCode = CodeType.CreateCode(CodeTypes.ACDR_CATCH_STATUS, CatchStatusCodes.LAN),
                        FAOIdentificationCode = CodeType.CreateCode(CodeTypes.FAO_AREA, "37.4.2"),
                        LandingCountryCode = CodeType.CreateCode(ListIDTypes.TERRITORY, CountryCodes.BGR.ToString()),
                        SovereigntyWaterCode = CodeType.CreateCode(CodeTypes.CR_SOV_WATERS, CrSovWaters.XEU.ToString()),
                        SpecifiedACDRCatch = new List<ACDRCatchType>
                        {
                            new ACDRCatchType
                            {
                                FAOSpeciesCode = CodeType.CreateCode(CodeTypes.FAO_SPECIES, specie.Code),
                                WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, specie.Quantity),
                                UsageCode = CodeType.CreateCode(CodeTypes.CR_LAND_IND, CrLandingIndicators.LAN_SOLD.ToString())
                            }
                        }
                    });

                    message.FLUXACDRReport.Add(report);
                }

                report = new FLUXACDRReportType
                {
                    TypeCode = CodeType.CreateCode(CodeTypes.ACDR_REPORT_TYPE, AcdReportTypes.ACDR_OTHER),
                    SpecifiedACDRReportedArea = new List<ACDRReportedAreaType>()
                };

                message.FLUXACDRReport.Add(report);

                var reportArea = new ACDRReportedAreaType
                {
                    CatchStatusCode = CodeType.CreateCode(CodeTypes.ACDR_CATCH_STATUS, CatchStatusCodes.LAN),
                    FAOIdentificationCode = CodeType.CreateCode(CodeTypes.FAO_AREA, "37.4.2"),
                    LandingCountryCode = CodeType.CreateCode(ListIDTypes.TERRITORY, CountryCodes.BGR.ToString()),
                    SovereigntyWaterCode = CodeType.CreateCode(CodeTypes.CR_SOV_WATERS, CrSovWaters.XEU.ToString()),
                    SpecifiedACDRCatch = new List<ACDRCatchType>()
                };

                report.SpecifiedACDRReportedArea.Add(reportArea);
                var otherCatches = reportArea.SpecifiedACDRCatch;

                foreach (var specie in result.Where(x => !quotaSpecies.ContainsKey(x.Code)))
                {
                    otherCatches.Add(new ACDRCatchType
                    {
                        FAOSpeciesCode = CodeType.CreateCode(CodeTypes.FAO_SPECIES, specie.Code),
                        WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, specie.Quantity),
                        UsageCode = CodeType.CreateCode(CodeTypes.CR_LAND_IND, CrLandingIndicators.LAN_SOLD.ToString())
                    });
                }

            }

            return message;
        }
    }
}
