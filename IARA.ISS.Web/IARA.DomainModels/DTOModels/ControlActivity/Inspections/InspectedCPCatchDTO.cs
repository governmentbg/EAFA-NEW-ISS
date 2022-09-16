namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectedCPCatchDTO
    {
        public int Id { get; set; }
        public int FishId { get; set; }
        public decimal CatchQuantity { get; set; }
        public bool IsDestroyed { get; set; }
        public bool IsDonated { get; set; }
        public bool IsStored { get; set; }
        public bool IsTaken { get; set; }
    }
}
