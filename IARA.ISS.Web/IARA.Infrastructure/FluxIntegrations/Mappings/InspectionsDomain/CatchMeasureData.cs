namespace IARA.Infrastructure.FluxIntegrations.Mappings.InspectionsDomain
{
    public class CatchMeasureData
    {
        public int? FishId { get; set; }

        public int? CatchCount { get; set; }

        public decimal? CatchQuantity { get; set; }

        public int? CatchZoneId { get; set; }

        public bool? Undersized { get; set; }

        public SubjectRoleEnum? Role { get; set; }

        public static List<CatchMeasureData> GetCatchMeasures(int inspectionId, IARADbContext db)
        {
            List<CatchMeasureData> result = (from catchMeasure in db.InspectionCatchMeasures
                                             join catchType in db.NcatchInspectionTypes on catchMeasure.CatchInspectionTypeId equals catchType.Id into catchTypeGrp
                                             from catchType in catchTypeGrp.DefaultIfEmpty()
                                             join page in db.InspectionLogBookPages on catchMeasure.InspectedLogBookPageId equals page.Id into pgrp
                                             from page in pgrp.DefaultIfEmpty()
                                             join ship in db.ShipsRegister on page.ShipId equals ship.Id into sgrp
                                             from ship in sgrp.DefaultIfEmpty()
                                             join unregShip in db.UnregisteredVessels on page.UnregisteredShipId equals unregShip.Id into usgrp
                                             from unregShip in usgrp.DefaultIfEmpty()
                                             where catchMeasure.IsActive
                                                   && catchMeasure.InspectionId == inspectionId
                                             orderby catchMeasure.Id descending
                                             select new CatchMeasureData
                                             {
                                                 FishId = catchMeasure.FishId,
                                                 CatchZoneId = catchMeasure.CatchZoneId,
                                                 Undersized = catchType == null ? null : new bool?(catchType.Code == nameof(CatchSizeCodesEnum.BMS)),
                                                 CatchQuantity = catchMeasure.CatchQuantity,
                                                 CatchCount = catchMeasure.CatchCount //TODO
                                             }).ToList();

            return result;
        }
    }
}
