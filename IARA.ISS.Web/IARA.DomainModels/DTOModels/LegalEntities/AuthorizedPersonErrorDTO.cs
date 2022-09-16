namespace IARA.DomainModels.DTOModels.LegalEntities
{
    public class AuthorizedPersonErrorDTO
    {
        public string EgnLnc { get; set; }
        public string Email { get; set; }
        public bool EgnAndEmailDontMatch { get; set; }
    }
}
