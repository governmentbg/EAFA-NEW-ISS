using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures
{
    public class InspectionSubjectPersonnelDto : UnregisteredPersonDto
    {
        public bool IsRegistered { get; set; }
        public int? EntryId { get; set; }
        public InspectedPersonType Type { get; set; }
        public InspectionSubjectAddressDto RegisteredAddress { get; set; }
    }
}
