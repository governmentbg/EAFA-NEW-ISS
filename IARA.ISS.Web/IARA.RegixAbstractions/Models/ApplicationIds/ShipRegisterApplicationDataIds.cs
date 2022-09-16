namespace IARA.RegixAbstractions.Models.ApplicationIds
{
    public class ShipRegisterApplicationDataIds : BaseRegixApplicationDataIds
    {
        public int ShipId { get; set; }
        public string Cfr { get; set; }
        public string Name { get; set; }
        public string RegLicenceNum { get; set; }
        public string RegLicensePublishVolume { get; set; }
        public string RegLicensePublishPage { get; set; }
        public int? VesselTypeId { get; set; }
        public string VesselTypeName { get; set; }
        public decimal GrossTonnage { get; set; }
        public decimal? NetTonnage { get; set; }
        public decimal TotalLength { get; set; }
        public decimal TotalWidth { get; set; }
        public decimal BoardHeight { get; set; }
        public decimal ShipDraught { get; set; }
        public decimal? LengthBetweenPerpendiculars { get; set; }
        public int FuelTypeId { get; set; }
        public string FuelTypeName { get; set; }
        public bool HasFishingCapacity { get; set; }
    }
}
