using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.QualifiedFrishersRegister
{
    public class QualifiedFisherDTO
    {
        public int Id { get; set; }
        public int? ApplicationId { get; set; }
        public PageCodeEnum PageCode { get; set; }
        public string RegistrationNum { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string Name { get; set; }
        public bool IsWithMaritimeEducation { get; set; }
        public string DiplomaOrExamLabel { get; set; } //set on frontend
        public string DiplomaOrExamNumber { get; set; }
        public int? DeliveryId { get; set; }
        public bool IsActive { get; set; }
    }
}
