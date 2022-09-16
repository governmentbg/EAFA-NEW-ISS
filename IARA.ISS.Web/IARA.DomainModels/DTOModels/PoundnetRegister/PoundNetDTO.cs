using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels
{
    public class PoundNetDTO
    {
        public int ID { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string SeasonType { get; set; }
        public string CategoryType { get; set; }
        public string Muncipality { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Status { get; set; }
        public PoundNetStatusesEnum StatusCode { get; set; }
        public bool IsActive { get; set; }
    }
}
