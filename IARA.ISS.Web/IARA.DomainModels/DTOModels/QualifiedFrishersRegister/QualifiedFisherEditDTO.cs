using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Duplicates;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.QualifiedFrishersRegister
{
    public class QualifiedFisherEditDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? RegistrationDate { get; set; }

        public bool IsOnlineApplication { get; set; }

        public string RegistrationNum { get; set; }

        public string Name { get; set; } // Това поле нужно ли е TODO ???

        public string EGN { get; set; } // Това поле нужно ли е TODO ???

        //[Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))] // TODO
        public bool IsWithMaritimeEducation { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? HasExam { get; set; }

        [RequiredIf(nameof(HasExam), "msgRequired", typeof(ErrorResources), true)]
        public int? ExamTerritoryUnitId { get; set; }

        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        [RequiredIf(nameof(HasPassedExam), "msgRequired", typeof(ErrorResources), true)]
        public string ExamProtocolNumber { get; set; }

        [RequiredIf(nameof(HasPassedExam), "msgRequired", typeof(ErrorResources), true)]
        public DateTime? ExamDate { get; set; }

        [RequiredIf(nameof(HasExam), "msgRequired", typeof(ErrorResources), true)]
        public bool? HasPassedExam { get; set; }

        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        [RequiredIf(nameof(HasExam), "msgRequired", typeof(ErrorResources), false)]
        public string DiplomaNumber { get; set; }

        [RequiredIf(nameof(HasExam), "msgRequired", typeof(ErrorResources), false)]
        public DateTime? DiplomaDate { get; set; }

        [StringLength(200, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        [RequiredIf(nameof(HasExam), "msgRequired", typeof(ErrorResources), false)]
        public string DiplomaIssuer { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Comments { get; set; }

        public int? ApplicationId { get; set; }

        public RegixPersonDataDTO SubmittedForRegixData { get; set; }

        public List<AddressRegistrationDTO> SubmittedForAddresses { get; set; }

        public List<DuplicatesEntryDTO> DuplicateEntries { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
