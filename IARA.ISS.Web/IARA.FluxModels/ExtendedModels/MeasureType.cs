using IARA.FluxModels.Enums;

namespace IARA.Flux.Models
{
    public partial class MeasureType
    {
        public static MeasureType CreatePowerKW(decimal value)
        {
            return new MeasureType
            {
                unitCode = nameof(FluxUnits.KWT),
                Value = value
            };
        }

        public static MeasureType CreateDraught(decimal value)
        {
            return new MeasureType
            {
                unitCode = nameof(FluxUnits.MTR),
                Value = value
            };
        }

        public static MeasureType CreateMeasure(FluxUnits type, decimal value)
        {
            return new MeasureType
            {
                unitCode = type.ToString(),
                Value = value
            };
        }
    }
}
