using IARA.FluxModels;
using IARA.FluxModels.Enums;

namespace IARA.Flux.Models
{
    public partial class VesselEquipmentCharacteristicType
    {
        public static VesselEquipmentCharacteristicType CreateBooleanEquipment(VesselEquipmentTypes equipmentType, bool value)
        {
            return new VesselEquipmentCharacteristicType
            {
                TypeCode = new CodeType
                {
                    listID = ListIDTypes.FLUX_VESSEL_EQUIP_TYPE,
                    Value = equipmentType.ToString()
                },
                ValueCode = new CodeType[]
                {
                    new CodeType
                    {
                        listID = ListIDTypes.BOOLEAN_TYPE,
                        Value = value ? "Y" : "N"
                    }
                }
            };
        }
    }
}
