using IARA.Mobile.Application.Interfaces.Mappings;
using IARA.Mobile.Pub.Domain.Entities.CatchRecords;
using System;

namespace IARA.Mobile.Pub.Application.DTObjects.CatchRecords.API
{
    public class CatchRecordTicketApiDto : IMapTo<CatchRecordTicket>
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string PersonFullName { get; set; }
        public string StatusName { get; set; }

        public void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap<CatchRecordTicketApiDto, CatchRecordTicket>();
        }
    }
}
