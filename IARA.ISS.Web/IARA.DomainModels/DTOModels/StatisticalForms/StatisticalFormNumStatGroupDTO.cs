using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.StatisticalForms
{
    public class StatisticalFormNumStatGroupDTO
    {
        public int Id { get; set; }

        public int? StatFormTypeId { get; set; }

        public string GroupName { get; set; }

        public NumericStatTypeGroupsEnum GroupType { get; set; }

        public List<StatisticalFormNumStatDTO> NumericStatTypes { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsActive { get; set; }
    }
}
