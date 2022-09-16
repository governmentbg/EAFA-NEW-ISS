using IARA.FluxModels;
using IARA.FluxModels.Enums;

namespace IARA.Flux.Models
{
    public partial class FishingGearType
    {
        public static FishingGearType CreateFishingGear(string type, VesselGearRoles gearRole)
        {
            return new FishingGearType
            {
                TypeCode = new CodeType
                {
                    listID = ListIDTypes.GEAR_TYPE,
                    Value = type
                },
                RoleCode = new CodeType[]
                {
                    new CodeType
                    {
                        listID = ListIDTypes.FLUX_VESSEL_GEAR_ROLE,
                        Value = gearRole.ToString()
                    }
                }
            };
        }
    }
}
