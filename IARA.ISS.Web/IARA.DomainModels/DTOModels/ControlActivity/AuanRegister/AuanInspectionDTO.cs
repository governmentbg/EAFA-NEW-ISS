using System;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.ControlActivity.AuanRegister
{
    public class AuanInspectionDTO 
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? UserId { get; set; }

        public DateTime? StartDate { get; set; }

        public int? TerritoryUnitId { get; set; }
    }
}
