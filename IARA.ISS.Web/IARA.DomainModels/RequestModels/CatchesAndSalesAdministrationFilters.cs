using System;
using System.Collections.Generic;

namespace IARA.DomainModels.RequestModels
{
    public class CatchesAndSalesAdministrationFilters : BaseRequestModel
    {
        public decimal? PageNumber { get; set; }

        public string OnlinePageNumber { get; set; }

        public int? LogBookTypeId { get; set; }

        public string LogBookNumber { get; set; }

        public decimal? DocumentNumber { get; set; }

        public int? ShipId { get; set; }

        public int? AquacultureId { get; set; }

        public int? RegisteredBuyerId { get; set; }

        public string OwnerEngEik { get; set; }

        public List<int> LogBookStatusIds { get; set; }

        public DateTime? LogBookValidityStartDate { get; set; }

        public DateTime? LogBookValidityEndDate { get; set; }

        public int? TerritoryUnitId { get; set; }

        public bool? FilterFishLogBookTeritorryUnitId { get; set; }

        public bool? FilterFirstSaleLogBookTeritorryUnitId { get; set; }

        public bool? FilterAdmissionLogBookTeritorryUnitId { get; set; }

        public bool? FilterTransportationLogBookTeritorryUnitId { get; set; }

        public bool? FilterAquacultureLogBookTeritorryUnitId { get; set; }

        public int? PersonId { get; set; }
    }
}
