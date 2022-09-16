using System;

namespace IARA.DomainModels.DTOModels.Mobile.CatchRecords
{
    public class CatchRecordTicketDTO
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string PersonFullName { get; set; }
        public string StatusName { get; set; }
    }
}
