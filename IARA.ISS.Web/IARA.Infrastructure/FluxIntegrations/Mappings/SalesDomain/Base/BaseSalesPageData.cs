using System;
using System.Collections.Generic;
using IARA.DataAccess;
using IARA.Flux.Models;
using IARA.FluxModels;
using IARA.FluxModels.Enums;

namespace IARA.Infrastructure.FluxIntegrations.Mappings.SalesDomain.Base
{
    internal abstract class BaseSalesPageData
    {
        protected string salesType;
        protected string pageNumber;
        protected string shipCfr;
        protected string shipName;
        protected string shipFlagCountryCode;
        protected string tripIdentifier;
        protected DateTime landingDateTime;
        protected string landingPortCode;
        protected string captainEgn;
        protected string captainFirstName;
        protected string captainMiddleName;
        protected string captainLastName;
        protected List<ProductData> products;

        public static BaseSalesPageData GetPageData(int id, IARADbContext db, string salesType)
        {
            return salesType switch
            {
                FluxSalesTypes.SN => FirstSalePageData.GetPageData(id, db),
                FluxSalesTypes.TOD => AdmissionPageData.GetPageData(id, db),
                FluxSalesTypes.TRD => TransportationPageData.GetPageData(id, db),
                _ => throw new ArgumentException(),
            };
        }

        public IDType CreateEuSalesId(int year)
        {
            string value = $"{SalesReportMapper.BULGARIA_CODE}-{salesType}-{year}-{pageNumber}";

            IDType result = IDType.CreateID(ListIDTypes.EU_SALES_ID, value);
            return result;
        }

        public static SalesEventType[] CreateSalesEvent(DateTime date, DateTime? date2 = null)
        {
            SalesEventType[] result;

            if (date2.HasValue)
            {
                result = new SalesEventType[2]
                {
                    new SalesEventType { OccurrenceDateTime = DateTimeType.BuildDateTime(date) },
                    new SalesEventType { OccurrenceDateTime = DateTimeType.BuildDateTime(date2.Value) }
                };
            }
            else
            {
                result = new SalesEventType[1]
                {
                    new SalesEventType { OccurrenceDateTime = DateTimeType.BuildDateTime(date) }
                };
            }

            return result;
        }

        public abstract SalesDocumentType CreateSalesDocument();

        protected abstract SalesPartyType[] CreateSalesParties();

        // Sales batch
        protected SalesBatchType[] CreateSalesBatch()
        {
            SalesBatchType result = new()
            {
                SpecifiedAAPProduct = CreateAAPProducts()
            };

            return new SalesBatchType[] { result };
        }

        protected AAPProductType[] CreateAAPProducts()
        {
            List<AAPProductType> result = new();

            foreach (ProductData product in products)
            {
                AAPProductType entry = CreateAAPProduct(product);
                result.Add(entry);
            }

            return result.ToArray();
        }

        protected static AAPProductType CreateAAPProduct(ProductData product)
        {
            AAPProductType result = new()
            {
                SpeciesCode = CodeType.CreateCode(ListIDTypes.FAO_SPECIES, product.SpeciesCode),
                WeightMeasure = MeasureType.CreateMeasure(FluxUnits.KGM, product.WeightKg),
                UsageCode = CodeType.CreateCode(ListIDTypes.PROD_USAGE, product.ProductUsageCode),
                AppliedAAPProcess = CreateAppliedAAPProcess(product),
                SpecifiedSizeDistribution = CreateSizeDistribution(product),
                TotalSalesPrice = new AmountType[]
                {
                    new AmountType
                    {
                        currencyID = SalesReportMapper.BULGARIA_CURRENCY_CODE,
                        Value = product.UnitPrice * product.WeightKg
                    }
                },
                OriginFLUXLocation = CreateProductLocation(product)
            };

            if (product.UnitCount.HasValue)
            {
                result.UnitQuantity = new QuantityType
                {
                    unitCode = nameof(FluxUnits.C62),
                    Value = product.UnitCount.Value
                };
            }

            return result;
        }

        protected static AAPProcessType[] CreateAppliedAAPProcess(ProductData product)
        {
            AAPProcessType result = new()
            {
                TypeCode = new CodeType[]
                {
                    CodeType.CreateCode(ListIDTypes.FISH_PRESENTATION, product.ProductPresentationCode),
                    CodeType.CreateCode(ListIDTypes.FISH_FRESHNESS, product.ProductFreshnessCode)
                }
            };

            return new AAPProcessType[] { result };
        }

        protected static SizeDistributionType CreateSizeDistribution(ProductData product)
        {
            SizeDistributionType result = new()
            {
                ClassCode = new CodeType[] { CodeType.CreateCode(ListIDTypes.FISH_SIZE_CLASS, product.FishSizeClassCode) },
                CategoryCode = product.FishSizeClassCode == "BMS"
                    ? CodeType.CreateCode(ListIDTypes.FISH_SIZE_CATEGORY, "N/A")
                    : CodeType.CreateCode(ListIDTypes.FISH_SIZE_CATEGORY, product.FishSizeCategoryCode)
            };

            return result;
        }

        protected static FLUXLocationType[] CreateProductLocation(ProductData product)
        {
            FLUXLocationType location = new()
            {
                TypeCode = CodeType.CreateCode(ListIDTypes.FLUX_LOCATION_TYPE, nameof(FluxLocationTypes.AREA)),
                ID = IDType.CreateID(ListIDTypes.FAO_AREA, product.CatchQuadrant)
            };

            return new FLUXLocationType[] { location };
        }

        // Fishing activity
        protected FishingActivityType[] CreateFishingActivity()
        {
            FishingActivityType result = new()
            {
                TypeCode = CodeType.CreateCode(ListIDTypes.FLUX_FA_TYPE, nameof(FaTypes.LANDING)),
                SpecifiedDelimitedPeriod = CreateLandingDelimitedPeriod(),
                RelatedVesselTransportMeans = CreateLandingVesselTransportMeans(),
                SpecifiedFishingTrip = !string.IsNullOrEmpty(tripIdentifier) ? CreateLandingFishingTrip() : null,
                RelatedFLUXLocation = CreateLandingLocation()
            };

            return new FishingActivityType[] { result };
        }

        protected DelimitedPeriodType[] CreateLandingDelimitedPeriod()
        {
            DelimitedPeriodType period = new()
            {
                StartDateTime = DateTimeType.BuildDateTime(landingDateTime)
            };

            return new DelimitedPeriodType[] { period };
        }

        protected FishingTripType CreateLandingFishingTrip()
        {
            FishingTripType type = new()
            {
                ID = new IDType[] { IDType.CreateID(ListIDTypes.EU_TRIP_ID, tripIdentifier) }
            };

            return type;
        }

        protected FLUXLocationType[] CreateLandingLocation()
        {
            FLUXLocationType location = new()
            {
                TypeCode = CodeType.CreateCode(ListIDTypes.FLUX_LOCATION_TYPE, nameof(FluxLocationTypes.LOCATION)),
                CountryID = IDType.CreateID(ListIDTypes.TERRITORY, SalesReportMapper.BULGARIA_CODE),
                ID = IDType.CreateID(ListIDTypes.LOCATION, landingPortCode)
            };

            return new FLUXLocationType[] { location };
        }

        // Vessel
        protected VesselTransportMeansType[] CreateLandingVesselTransportMeans()
        {
            VesselTransportMeansType result = new()
            {
                ID = new IDType[] { IDType.CreateCFR(shipCfr) },
                Name = new TextType[] { TextType.CreateText(shipName) },
                RegistrationVesselCountry = new VesselCountryType
                {
                    ID = IDType.CreateID(ListIDTypes.TERRITORY, shipFlagCountryCode)
                },
                SpecifiedContactParty = CreateLandingVesselContactParty()
            };

            return new VesselTransportMeansType[] { result };
        }

        protected ContactPartyType[] CreateLandingVesselContactParty()
        {
            ContactPartyType[] result = new ContactPartyType[]
            {
                new ContactPartyType
                {
                    RoleCode = new CodeType[] { CodeType.CreateCode(ListIDTypes.FLUX_CONTACT_ROLE, nameof(FluxContactRoles.OPERATOR)) },
                    SpecifiedContactPerson = new ContactPersonType[]
                    {
                        new ContactPersonType
                        {
                            GivenName = captainFirstName,
                            MiddleName = captainMiddleName,
                            FamilyName = captainLastName
                        }
                    }
                }
            };

            return result;
        }
    }
}
