using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class CommonLogBookPageDataDTO
    {
        public List<PossibleLogBooksForPageDTO> PossibleLogBooks { get; set; }

        public int? OriginDeclarationId { get; set; }

        public string OriginDeclarationNumber { get; set; }

        public DateTime? OriginDeclarationDate { get; set; }

        public int? TransportationDocumentId { get; set; }

        public decimal? TransportationDocumentNumber { get; set; }

        public DateTime? TransportationDocumentDate { get; set; }

        public int? AdmissionDocumentId { get; set; }

        public decimal? AdmissionDocumentNumber { get; set; }

        public DateTime? AdmissionHandoverDate { get; set; }

        public int? ShipId { get; set; }

        public string CaptainName { get; set; }

        public string UnloadingInformation { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsImportNotByShip { get; set; }

        [RequiredIf(nameof(IsImportNotByShip), "msgRequired", typeof(ErrorResources), true)]
        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string PlaceOfImport { get; set; }

        public string VendorName { get; set; }
    }
}
