using System;
using System.Collections.Generic;
using System.Text;

namespace IARA.DomainModels.DTOModels.Application
{
    public class ApplicationPaymentInformationDTO
    {
        public int Id { get; set; }

        public string PaymentType { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string PaymentStatus { get; set; }

        public string ReferenceNumber { get; set; }

        public DateTime? LastUpdateDate { get; set; }

        public PaymentSummaryDTO PaymentSummary { get; set; }
    }
}
