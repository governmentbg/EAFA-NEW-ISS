using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Reports
{
    public class ReportDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ReportGroupId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Name { get; set; }

        [StringLength(1000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Description { get; set; }
        [StringLength(20, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public string ReportType { get; set; }
        public DateTime? LastRunDateTime { get; set; }
        public string LastRunUsername { get; set; }
        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string IconName { get; set; }
        public int? LastRunDurationSec { get; set; }
        public List<ReportParameterDTO> Parameters { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [StringLength(200, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Code { get; set; }

        public List<NomenclatureDTO> Roles { get; set; }
        public List<NomenclatureDTO> Users { get; set; }
        public string ReportSQL { get; set; }
    }
}
