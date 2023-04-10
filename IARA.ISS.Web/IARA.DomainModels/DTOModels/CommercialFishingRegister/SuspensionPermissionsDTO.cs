namespace IARA.DomainModels.DTOModels.CommercialFishingRegister
{
    public class SuspensionPermissionsDTO
    {
        public bool CanReadSuspensions { get; set; }

        public bool CanAddSuspensions { get; set; }

        public bool CanEditSuspensions { get; set; }

        public bool CanDeleteSuspensions { get; set; }

        public bool CanRestoreSuspensions { get; set; }
    }
}
