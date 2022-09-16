using IARA.Mobile.Domain.Interfaces;

namespace IARA.Mobile.Pub.Application.DTObjects.FishingTickets.LocalDb
{
    public class AssociationSelectDto : ISelectProperty
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
