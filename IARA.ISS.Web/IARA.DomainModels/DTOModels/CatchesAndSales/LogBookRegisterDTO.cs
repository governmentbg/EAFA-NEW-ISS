using System;
using System.Collections.Generic;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class LogBookRegisterDTO
    {
        public int Id { get; set; }

        public string Number { get; set; }

        public string Type { get; set; }

        public LogBookTypesEnum TypeCode { get; set; }

        public string Name { get; set; }

        public long StartPageNum { get; set; }

        public long EndPageNum { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime? FinishDate { get; set; }

        public string StatusName { get; set; }

        public bool IsLogBookFinished { get; set; }

        public bool IsOnline { get; set; }

        public LogBookPagePersonTypesEnum? OwnerType { get; set; }

        public List<ShipLogBookPageRegisterDTO> ShipPages { get; set; }

        public List<AdmissionLogBookPageRegisterDTO> AdmissionPages { get; set; }

        public List<TransportationLogBookPageRegisterDTO> TransportationPages { get; set; }

        public List<FirstSaleLogBookPageRegisterDTO> FirstSalePages { get; set; }

        public List<AquacultureLogBookPageRegisterDTO> AquaculturePages { get; set; }
    }
}
