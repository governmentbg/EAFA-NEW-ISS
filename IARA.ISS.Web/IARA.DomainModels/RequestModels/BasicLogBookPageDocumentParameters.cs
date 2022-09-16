using System.ComponentModel.DataAnnotations;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.RequestModels
{
    public class BasicLogBookPageDocumentParameters
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public LogBookPageDocumentTypesEnum? DocumentType { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ShipLogBookPageId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? DocumentNumber { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? LogBookId { get; set; }
    }
}
