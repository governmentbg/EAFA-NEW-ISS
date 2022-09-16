using System.Collections.Generic;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.RolesRegister
{
    public class PermissionGroupDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ParentGroup { get; set; }

        public NomenclatureDTO ReadAllPermission { get; set; }

        public NomenclatureDTO ReadPermission { get; set; }

        public NomenclatureDTO AddPermission { get; set; }

        public NomenclatureDTO EditPermission { get; set; }

        public NomenclatureDTO DeletePermission { get; set; }

        public NomenclatureDTO RestorePermission { get; set; }

        public List<NomenclatureDTO> OtherPermissions { get; set; }
    }
}
