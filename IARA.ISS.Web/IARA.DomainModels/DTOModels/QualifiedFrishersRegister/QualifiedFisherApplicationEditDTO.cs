using System;
using System.Collections.Generic;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.QualifiedFrishersRegister
{
    public class QualifiedFisherApplicationEditDTO : QualifiedFisherRegixDataDTO
    {
        public LetterOfAttorneyDTO LetterOfAttorney { get; set; }
        public List<FileInfoDTO> Files { get; set; }
        public string Name { get; set; }
        public string EGN { get; set; }
        public bool? HasExam { get; set; }
        public int? ExamTerritoryUnitId { get; set; }
        public string Comments { get; set; }
        public ApplicationPaymentInformationDTO PaymentInformation { get; set; }
        public ApplicationBaseDeliveryDTO DeliveryData { get; set; }
        public bool IsPaid { get; set; }
        public bool HasDelivery { get; set; }
        public bool IsOnlineApplication { get; set; }
    }
}
