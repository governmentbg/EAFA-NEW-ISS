using System;
using IARA.FluxModels;
using IARA.FluxModels.Enums;

namespace IARA.Flux.Models
{
    public partial class VesselAdministrativeCharacteristicType
    {
        public static VesselAdministrativeCharacteristicType CreateAdministrativeCharacteristic(VesselAdminTypes type, string value)
        {
            CodeType valueCode = null;

            switch (type)
            {
                case VesselAdminTypes.SEG:
                    valueCode = CodeType.CreateCode(ListIDTypes.VESSEL_SEGMENT, value);
                    break;
                case VesselAdminTypes.EXPORT:
                    valueCode = CodeType.CreateCode(ListIDTypes.VESSEL_EXPORT_TYPE, value);
                    break;
                case VesselAdminTypes.AID:
                    valueCode = CodeType.CreateCode(ListIDTypes.VESSEL_PUBLIC_AID_TYPE, value);
                    break;
                case VesselAdminTypes.EIS:
                case VesselAdminTypes.PURCHASE_YEAR:
                    throw new ArgumentException("Invalid for string argument. Use DateTime");
                case VesselAdminTypes.LICENCE:
                    throw new ArgumentException("Invalid for string argument. Use bool");
            }

            return new VesselAdministrativeCharacteristicType
            {
                TypeCode = CodeType.CreateCode(ListIDTypes.FLUX_VESSEL_ADMIN_TYPE, type.ToString()),
                ValueCode = valueCode
            };
        }

        public static VesselAdministrativeCharacteristicType CreateAdministrativeCharacteristic(VesselAdminTypes type, bool value)
        {
            if (type == VesselAdminTypes.LICENCE)
            {
                return new VesselAdministrativeCharacteristicType
                {
                    TypeCode = CodeType.CreateCode(ListIDTypes.FLUX_VESSEL_ADMIN_TYPE, type.ToString()),
                    ValueCode = CodeType.CreateCode(ListIDTypes.BOOLEAN_TYPE, value ? "Y" : "N")
                };
            }

            throw new ArgumentException("Invalid for bool argument.");
        }

        public static VesselAdministrativeCharacteristicType CreateAdministrativeCharacteristic(VesselAdminTypes type, DateTime value)
        {
            switch (type)
            {
                case VesselAdminTypes.EIS:
                    return new VesselAdministrativeCharacteristicType
                    {
                        TypeCode = CodeType.CreateCode(ListIDTypes.FLUX_VESSEL_ADMIN_TYPE, type.ToString()),
                        ValueDateTime = value
                    };
                case VesselAdminTypes.PURCHASE_YEAR:
                    return new VesselAdministrativeCharacteristicType
                    {
                        TypeCode = CodeType.CreateCode(ListIDTypes.FLUX_VESSEL_ADMIN_TYPE, type.ToString()),
                        ValueDateTime = value
                    };
            }

            throw new ArgumentException("Invalid for DateTime argument.");
        }
    }
}
