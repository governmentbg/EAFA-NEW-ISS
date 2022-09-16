namespace IARA.DomainModels.DTOModels.Application
{
    public class ApplicationTariffDTO
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string BasedOnPlea { get; set; }
    }
}
