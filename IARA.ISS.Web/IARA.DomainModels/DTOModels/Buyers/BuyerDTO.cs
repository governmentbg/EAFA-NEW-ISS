using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.Buyers
{
    public class BuyerDTO
    {
        public int? Id { get; set; }
        public int ApplicationId { get; set; }
        public BuyerStatusesEnum Status { get; set; }
        public PageCodeEnum PageCode { get; set; }
        public string SubmittedForName { get; set; }
        /// <summary>
        /// Име на магазин и/или номер на превозно средство, което е регистрирано
        /// </summary>
        public string SubjectNames { get; set; }
        public string BuyerTypeName { get; set; }
        public string BuyerStatusName { get; set; }
        public string UrorrNumber { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }
        public string Comments { get; set; }
    }
}
