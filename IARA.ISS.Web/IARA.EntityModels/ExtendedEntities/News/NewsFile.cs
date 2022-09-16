using IARA.EntityModels.Interfaces;

namespace IARA.EntityModels.Entities
{
	  public partial class NewsFile : IAuditEntity, ISoftDeletable, IIdentityRecord, IFileEntity<News>
	  {
	  }
}