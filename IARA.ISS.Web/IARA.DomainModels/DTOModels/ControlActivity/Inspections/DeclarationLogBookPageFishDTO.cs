namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class DeclarationLogBookPageFishDTO
    {
        public int Id { get; set; }
        public int LogBookId { get; set; }
        public int FishId { get; set; }
        public int? PresentationId { get; set; }
        public decimal Quantity { get; set; }
    }
}
