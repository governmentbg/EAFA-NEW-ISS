using IARA.Mobile.Application.DTObjects.Nomenclatures;
using System;

namespace IARA.Mobile.Application.DTObjects.Profile.API
{
    public class RoleApiDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime AccessValidFrom { get; set; }
        public DateTime AccessValidTo { get; set; }
        public NomenclatureDto SelectedUserRole { get; set; }
    }
}
