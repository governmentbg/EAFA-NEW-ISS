using IARA.Mobile.Domain.Models;

namespace IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API
{
    public class Under14AdditionalTicketDto
    {
        public PersonDto Person { get; set; }
        public FileModel Photo { get; set; }
        public FileModel BirthCertificate { get; set; }
    }
}
