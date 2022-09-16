using IARA.EntityModels.Interfaces;

namespace IARA.EntityModels.Entities
{
	  public partial class ApplicationChangeHistoryFile : IAuditEntity, ISoftDeletable, IIdentityRecord, IFileEntity<ApplicationChangeHistory>
	  {
	  }
}