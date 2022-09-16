using IARA.EntityModels.Interfaces;

namespace IARA.EntityModels.Entities
{
	  public partial class InspectionRegisterFile : IAuditEntity, ISoftDeletable, IIdentityRecord, IFileEntity<InspectionRegister>
	  {
	  }
}