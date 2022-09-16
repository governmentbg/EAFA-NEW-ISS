using IARA.EntityModels.Interfaces;

namespace IARA.EntityModels.Entities
{
	  public partial class PermitRegisterFile : IAuditEntity, ISoftDeletable, IIdentityRecord, IFileEntity<PermitRegister>
	  {
	  }
}