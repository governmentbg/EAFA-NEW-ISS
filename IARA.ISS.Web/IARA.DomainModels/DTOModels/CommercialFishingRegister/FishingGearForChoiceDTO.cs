namespace IARA.DomainModels.DTOModels.CommercialFishingRegister
{
    public class FishingGearForChoiceDTO
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public int Count { get; set; }

        public string MarksNumbers { get; set; }

        public decimal? Length { get; set; }

        public decimal? NetEyeSize { get; set; }

        public string Description { get; set; }

        public bool? IsChecked { get; set; }
    }
}
