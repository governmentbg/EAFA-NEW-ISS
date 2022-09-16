using System;
using System.Linq;
using IARA.Flux.Models;
using IARA.FluxModels;
using IARA.FluxModels.Enums;

namespace IARA.Infrastructure.FluxIntegrations.Mappings.SalesDomain.Base
{
    internal abstract class FirstSaleAdmissionPageData : BaseSalesPageData
    {
        protected DateTime date;
        protected string location;

        public override SalesDocumentType CreateSalesDocument()
        {
            SalesDocumentType document = new()
            {
                ID = new IDType[] { CreateEuSalesId(date.Year) },
                CurrencyCode = CodeType.CreateCode(ListIDTypes.TERRITORY_CURR, SalesReportMapper.BULGARIA_CURRENCY_CODE),
                SpecifiedSalesEvent = CreateSalesEvent(date),
                TotalSalesPrice = CreateTotalSalePrice(),
                SpecifiedSalesParty = CreateSalesParties(),
                SpecifiedFishingActivity = CreateFishingActivity(),
                SpecifiedFLUXLocation = CreateLocation(),
                SpecifiedSalesBatch = CreateSalesBatch()
            };

            return document;
        }

        private AmountType[] CreateTotalSalePrice()
        {
            AmountType amount = new()
            {
                currencyID = SalesReportMapper.BULGARIA_CURRENCY_CODE,
                Value = products.Sum(x => x.UnitPrice * x.WeightKg)
            };

            return new AmountType[] { amount };
        }

        private FLUXLocationType[] CreateLocation()
        {
            FLUXLocationType result = new()
            {
                TypeCode = CodeType.CreateCode(ListIDTypes.FLUX_LOCATION_TYPE, nameof(FluxLocationTypes.AREA)),
                CountryID = IDType.CreateID(ListIDTypes.TERRITORY, SalesReportMapper.BULGARIA_CODE),
                ID = IDType.CreateID(ListIDTypes.FAO_AREA, location)
            };

            return new FLUXLocationType[] { result };
        }
    }
}
