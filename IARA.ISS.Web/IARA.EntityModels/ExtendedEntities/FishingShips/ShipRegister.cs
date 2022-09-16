using IARA.EntityModels.Interfaces;

namespace IARA.EntityModels.Entities
{
	  public partial class ShipRegister : IAuditEntity, IIdentityRecord, INullableApplicationRegisterValidityEntity, IValidity, ICancellableEntity
	  {
	  }
}