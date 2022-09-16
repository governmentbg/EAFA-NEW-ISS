using IARA.EntityModels.Interfaces;

namespace IARA.EntityModels.Entities
{
	  public partial class LegalFile : IAuditEntity, ISoftDeletable, IIdentityRecord, IFileEntity<Legal>
	  {
	  }
}