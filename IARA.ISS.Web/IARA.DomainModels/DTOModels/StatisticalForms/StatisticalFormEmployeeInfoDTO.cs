using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.StatisticalForms
{
    public class StatisticalFormEmployeeInfoDTO
    {
        public int Id { get; set; }

        public int? GroupId { get; set; }

        public int? StatFormId { get; set; }
        //[Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? MenWithPay { get; set; }
        //[Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? MenWithoutPay { get; set; }
        //[Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? WomenWithPay { get; set; }
        //[Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? WomenWithoutPay { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public short? OrderNum { get; set; }
        public bool IsActive { get; set; }
    }
}
