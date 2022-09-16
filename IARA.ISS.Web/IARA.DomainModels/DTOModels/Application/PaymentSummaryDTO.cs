using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.Application
{
    public class PaymentSummaryDTO
    {
        public List<PaymentTariffDTO> Tariffs { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
