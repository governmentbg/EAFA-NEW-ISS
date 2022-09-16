namespace IARA.DomainModels.DTOModels.ShipsRegister
{
    public class ShipRegisterUserDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string EgnLncEik { get; set; }

        public string PermitLicenceType { get; set; }

        public string GroundsForUse { get; set; }

        public bool IsActive { get; set; }
    }
}
