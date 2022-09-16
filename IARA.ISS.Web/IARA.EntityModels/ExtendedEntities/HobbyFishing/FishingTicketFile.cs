using IARA.EntityModels.Interfaces;

namespace IARA.EntityModels.Entities
{
	  public partial class FishingTicketFile : IAuditEntity, ISoftDeletable, IIdentityRecord, IFileEntity<FishingTicket>
	  {
	  }
}