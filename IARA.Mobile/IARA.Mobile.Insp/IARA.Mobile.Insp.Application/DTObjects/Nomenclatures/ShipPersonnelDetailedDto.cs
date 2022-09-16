using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures
{
    public class ShipPersonnelDetailedDto
    {
        public int Id { get; set; }
        public int? EntryId { get; set; }
        public EgnLncDto EgnLnc { get; set; }
        public string Eik { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public bool IsLegal { get; set; }
        public InspectedPersonType Type { get; set; }
        public InspectionSubjectAddressDto Address { get; set; }
    }
}
