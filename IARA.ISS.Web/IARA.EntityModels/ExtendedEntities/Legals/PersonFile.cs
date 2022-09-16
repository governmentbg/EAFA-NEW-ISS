using IARA.EntityModels.Interfaces;

namespace IARA.EntityModels.Entities
{
	  public partial class PersonFile : IAuditEntity, ISoftDeletable, IIdentityRecord, IFileEntity<Person>
	  {
	  }
}