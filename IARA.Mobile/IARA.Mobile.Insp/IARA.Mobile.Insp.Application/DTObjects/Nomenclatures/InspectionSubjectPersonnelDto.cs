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

        public static InspectionSubjectPersonnelDto CreateCopy(InspectionSubjectPersonnelDto dto, InspectedPersonType type, bool isLegal)
        {
            return new InspectionSubjectPersonnelDto()
            {
                IsRegistered = dto.IsRegistered,
                EntryId = dto.EntryId,
                Type = type,
                RegisteredAddress = dto.RegisteredAddress,

                Id = dto.Id,
                FirstName = dto.FirstName,
                MiddleName = dto.MiddleName,
                LastName = dto.LastName,
                Address = dto.Address,
                HasBulgarianAddressRegistration = dto.HasBulgarianAddressRegistration,
                EgnLnc = dto.EgnLnc,
                Eik = dto.Eik,
                IsLegal = isLegal,
                CitizenshipId = dto.CitizenshipId,
                Comment = dto.Comment,
                IsActive = dto.IsActive
            };
        }
    }
}
