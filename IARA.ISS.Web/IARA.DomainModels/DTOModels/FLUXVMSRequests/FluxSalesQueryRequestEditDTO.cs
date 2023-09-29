using System;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.FLUXVMSRequests
{
    public class FluxSalesQueryRequestEditDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public FluxSalesTypeEnum? QueryType { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? DateTimeFrom { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? DateTimeTo { get; set; }

        public string VesselCFR { get; set; }

        public string SalesID { get; set; }

        public string TripID { get; set; }
    }
}
