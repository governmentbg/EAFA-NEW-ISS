using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class VesselDuringInspectionDTO : VesselDTO
    {
        public int? Id { get; set; }
        public LocationDTO Location { get; set; }
        public int? CatchZoneId { get; set; }

        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string LocationDescription { get; set; }

        public int? ShipAssociationId { get; set; }
    }
}
