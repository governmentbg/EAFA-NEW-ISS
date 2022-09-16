using System.Collections.Generic;
using System.Linq;
using IARA.DataAccess;
using IARA.EntityModels.Entities;

namespace IARA.Infrastructure.FluxIntegrations.Mappings.SalesDomain.Base
{
    public class ProductData
    {
        public string SpeciesCode { get; set; }

        public decimal WeightKg { get; set; }

        public int? UnitCount { get; set; }

        public string ProductUsageCode { get; set; }

        public string ProductFreshnessCode { get; set; }

        public string ProductPresentationCode { get; set; }

        public string FishSizeClassCode { get; set; }

        public string FishSizeCategoryCode { get; set; }

        public decimal UnitPrice { get; set; }

        public string CatchQuadrant { get; set; }

        public static List<ProductData> GetFirstSaleProductData(IARADbContext db, int pageId)
        {
            IQueryable<LogBookPageProduct> query = from product in db.LogBookPageProducts
                                                   where product.FirstSaleLogBookPageId == pageId
                                                   select product;

            List<ProductData> result = GetProducts(db, query);
            return result;
        }

        public static List<ProductData> GetAdmissionProductData(IARADbContext db, int pageId)
        {
            IQueryable<LogBookPageProduct> query = from product in db.LogBookPageProducts
                                                   where product.AdmissionLogBookPageId == pageId
                                                   select product;

            List<ProductData> result = GetProducts(db, query);
            return result;
        }

        public static List<ProductData> GetTransportationProductData(IARADbContext db, int pageId)
        {
            IQueryable<LogBookPageProduct> query = from product in db.LogBookPageProducts
                                                   where product.TransportationLogBookPageId == pageId
                                                   select product;

            List<ProductData> result = GetProducts(db, query);
            return result;
        }

        private static List<ProductData> GetProducts(IARADbContext db, IQueryable<LogBookPageProduct> query)
        {
            List<ProductData> result = (from product in query
                                        join fish in db.Nfishes on product.FishId equals fish.Id
                                        join purpose in db.NfishSalePurposes on product.ProductPurposeId equals purpose.Id
                                        join freshness in db.NfishFreshnesses on product.ProductFreshnessId equals freshness.Id
                                        join presentation in db.NfishPresentations on product.ProductPresentationId equals presentation.Id
                                        join sizeCategory in db.NfishSizeCategories on product.FishSizeCategoryId equals sizeCategory.Id
                                        join originDeclarationFish in db.OriginDeclarationFish on product.OriginDeclarationFishId equals originDeclarationFish.Id
                                        join catchRecordFish in db.CatchRecordFish on originDeclarationFish.CatchRecordFishId equals catchRecordFish.Id
                                        join catchSize in db.NfishSizes on catchRecordFish.CatchSizeId equals catchSize.Id
                                        join catchZone in db.NcatchZones on catchRecordFish.CatchZoneId equals catchZone.Id
                                        select new ProductData
                                        {
                                            SpeciesCode = fish.Code,
                                            WeightKg = product.QuantityKg,
                                            UnitCount = product.UnitCount,
                                            ProductUsageCode = purpose.Code,
                                            ProductFreshnessCode = freshness.Code,
                                            ProductPresentationCode = presentation.Code,
                                            FishSizeClassCode = catchSize.Code,
                                            FishSizeCategoryCode = sizeCategory.Code,
                                            UnitPrice = product.UnitPrice,
                                            CatchQuadrant = catchZone.Gfcmquadrant
                                        }).ToList();

            return result;
        }
    }
}
