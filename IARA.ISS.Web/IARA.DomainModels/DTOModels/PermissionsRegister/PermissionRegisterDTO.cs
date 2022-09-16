namespace IARA.DomainModels.DTOModels.PermissionsRegister
{
    public class PermissionRegisterDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Group { get; set; }

        public string Type { get; set; }

        public int RolesCount { get; set; }
    }
}
