using IARA.EntityModels.Interfaces;

namespace IARA.EntityModels.Entities
{
	  public partial class ShipRegisterFile : IAuditEntity, ISoftDeletable, IIdentityRecord, IFileEntity<ShipRegister>
	  {
	  }
}