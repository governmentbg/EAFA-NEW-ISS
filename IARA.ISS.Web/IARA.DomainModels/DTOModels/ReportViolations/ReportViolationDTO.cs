using System;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.ReportViolations
{
    public class ReportViolationDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public NomenclatureDTO SignalType { get; set; }

        [StringLength(1000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Description { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Phone { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? Date { get; set; }

        public LocationDTO Location { get; set; }
    }
}
