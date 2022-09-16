using IARA.FluxModels;
using IARA.FluxModels.Enums;

namespace IARA.Flux.Models
{
    public partial class CodeType
    {
        public static CodeType CreateVesselReportType(string value)
        {
            return CreateCode(ListIDTypes.FLUX_VESSEL_REPORT_TYPE, value);
        }

        public static CodeType CreatePurpose(ReportPurposeCodes value)
        {
            return CreateCode(ListIDTypes.FLUX_GP_PURPOSE, ((int)value).ToString());
        }

        public static CodeType CreateVesselEvent(string value)
        {
            return CreateCode(ListIDTypes.VESSEL_EVENT, value);
        }

        public static CodeType CreateVesselType(string value)
        {
            return CreateCode(ListIDTypes.VESSEL_TYPE, value);
        }

        public static CodeType CreateVesselEngineType(VesselEngineTypes type)
        {
            return CreateCode(ListIDTypes.FLUX_VESSEL_ENGINE_ROLE, type.ToString());
        }

        public static CodeType HullMaterial(string type)
        {
            return CreateCode(ListIDTypes.VESSEL_HULL_TYPE, type);
        }

        public static CodeType DimensionType(DimensionTypes value)
        {
            return CreateCode(ListIDTypes.FLUX_VESSEL_DIM_TYPE, value.ToString());
        }

        public static CodeType CreateCode(string type, string value)
        {
            return new CodeType
            {
                listID = type,
                Value = value
            };
        }
    }
}
