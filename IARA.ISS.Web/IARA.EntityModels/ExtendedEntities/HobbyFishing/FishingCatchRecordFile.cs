using IARA.EntityModels.Interfaces;

namespace IARA.EntityModels.Entities
{
	  public partial class FishingCatchRecordFile : IAuditEntity, ISoftDeletable, IIdentityRecord, IFileEntity<FishingCatchRecord>
	  {
	  }
}