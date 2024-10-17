using System.Collections.Generic;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.RolesRegister
{
    public class ReportRoleGroupDTO 
    {
        public int Id { get; set; }

        public int GroupId { get; set; }

        public string ParentGroup { get; set; }

        public bool Expanded { get; set; }

        public List<NomenclatureDTO> Reports { get; set; }
    }
}
