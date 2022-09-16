using System.Collections.Generic;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.Application
{
    public class ApplicationSubmissionDTO
    {
        public RegixPersonDataDTO SubmittedByPerson { get; set; }

        public FileInfoDTO SubmittedByPersonPhoto { get; set; }

        public List<AddressRegistrationDTO> SubmittedByPersonAddresses { get; set; }

        public LetterOfAttorneyDTO SubmittedByLetterOfAttorney { get; set; }

        public RegixPersonDataDTO SubmittedForPerson { get; set; }

        public FileInfoDTO SubmittedForPersonPhoto { get; set; }

        public RegixLegalDataDTO SubmittedForLegal { get; set; }

        public List<AddressRegistrationDTO> SubmittedForAddresses { get; set; }

        public string ApplicationDraft { get; set; }
    }
}
