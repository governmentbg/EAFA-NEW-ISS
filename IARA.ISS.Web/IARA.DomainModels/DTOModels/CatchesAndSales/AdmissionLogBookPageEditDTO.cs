using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Files;
using Newtonsoft.Json;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class AdmissionLogBookPageEditDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? LogBookId { get; set; }

        public int? LogBookPermitLicenseId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? PageNumber { get; set; }

        public string Status { get; set; } // For UI

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public CommonLogBookPageDataDTO CommonData { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? HandoverDate { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string StorageLocation { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public LogBookPagePersonDTO AcceptingPerson { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<LogBookPageProductDTO> Products { get; set; }

        /// <summary>
        /// Продуктите, идващи от декларация за произход/документ за превоз, от които могат да се създават продуктите
        /// </summary>
        public List<LogBookPageProductDTO> OriginalPossibleProducts { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
