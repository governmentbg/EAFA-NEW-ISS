using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.StatisticalForms
{
    public class StatisticalFormEmployeeInfoGroupDTO
    {
        public int Id { get; set; }
        public int? StatFormTypeId { get; set; }
        public string GroupName { get; set; }
        public bool IsActive { get; set; }
        public List<StatisticalFormEmployeeInfoDTO> EmployeeTypes { get; set; }
    }
}
