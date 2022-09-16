using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class LogBookEditDTO
    {
        public int? LogBookId { get; set; }

        public int LogBookTypeId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? StatusId { get; set; }

        public LogBookPagePersonTypesEnum? OwnerType { get; set; }

        public string LogbookNumber { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? IssueDate { get; set; }

        public DateTime? FinishDate { get; set; }

        [RequiredIf(nameof(IsOnline), "msgRequired", typeof(ErrorResources), false)]
        public long? StartPageNumber { get; set; }

        [RequiredIf(nameof(IsOnline), "msgRequired", typeof(ErrorResources), false)]
        public long? EndPageNumber { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsOnline { get; set; }

        public bool LogBookIsActive { get; set; }

        /// <summary>
        /// For UI only
        /// </summary>
        public bool IsActive { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal Price { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Comment { get; set; }

        public bool? HasError { get; set; }

        public List<TransportationLogBookPageRegisterDTO> TransportationPagesAndDeclarations { get; set; }

        public List<AdmissionLogBookPageRegisterDTO> AdmissionPagesAndDeclarations { get; set; }

        public List<FirstSaleLogBookPageRegisterDTO> FirstSalePages { get; set; }

        public List<AquacultureLogBookPageRegisterDTO> AquaculturePages { get; set; }
    }
}
