using System.ComponentModel.DataAnnotations;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.DomainModels.RequestModels
{
    public class CommonLogBookPageDataParameters
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public LogBookTypesEnum? LogBookType { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? PageNumberToAdd { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? LogBookId { get; set; }

        public string OriginDeclarationNumber { get; set; }

        public decimal? TransportationDocumentNumber { get; set; }

        public decimal? AdmissionDocumentNumber { get; set; }
    }
}
