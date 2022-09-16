using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.Vehicles
{
    public class PatrolVehiclesEditDTO
    {
        public int Id { get; set; }
        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Name { get; set; }
        public int? FlagCountryId { get; set; }
        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string ExternalMark { get; set; }
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? PatrolVehicleTypeId { get; set; }
        [StringLength(20, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Cfr { get; set; }
        [StringLength(20, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Uvi { get; set; }
        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string IrcscallSign { get; set; }
        [StringLength(20, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Mmsi { get; set; }
        public int? VesselTypeId { get; set; }
        public int? InstitutionId { get; set; }
    }
}
