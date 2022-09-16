using IARA.EntityModels.Interfaces;

namespace IARA.EntityModels.Entities
{
	  public partial class ApplicationFile : IAuditEntity, ISoftDeletable, IIdentityRecord, IFileEntity<Application>
	  {
	  }
}