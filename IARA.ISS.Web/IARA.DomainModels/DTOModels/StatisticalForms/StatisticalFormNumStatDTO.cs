using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.StatisticalForms
{
    public class StatisticalFormNumStatDTO
    {
        public int Id { get; set; }

        public int GroupId { get; set; }

        public int? StatFormId { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string DataType { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? StatValue { get; set; }

        public short? OrderNum { get; set; }

        public bool IsActive { get; set; }
    }
}
