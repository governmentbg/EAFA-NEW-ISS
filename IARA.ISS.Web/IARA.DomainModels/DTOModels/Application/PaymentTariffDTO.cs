namespace IARA.DomainModels.DTOModels.Application
{
    public class PaymentTariffDTO
    {
        public int TariffId { get; set; }

        /// <summary>
        /// Only for UI (set in UI)
        /// </summary>
        public string TariffName { get; set; }


        /// <summary>
        /// Only for UI (set in UI)
        /// </summary>
        public string TariffDescription { get; set; }

        /// <summary>
        /// Only for UI (set in UI)
        /// </summary>
        public string TariffBasedOnPlea { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Price { get; set; }

        public bool IsCalculated { get; set; }
    }
}
