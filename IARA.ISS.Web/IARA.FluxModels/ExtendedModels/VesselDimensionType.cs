using IARA.FluxModels.Enums;

namespace IARA.Flux.Models
{
    public partial class VesselDimensionType
    {

        public static VesselDimensionType LengthOverall(decimal value)
        {
            return new VesselDimensionType
            {
                TypeCode = CodeType.DimensionType(DimensionTypes.LOA),
                ValueMeasure = new MeasureType
                {
                    unitCode = nameof(FluxUnits.MTR),
                    Value = value
                }
            };
        }

        public static VesselDimensionType LengthBetweenPerpendiculars(decimal value)
        {
            return new VesselDimensionType
            {
                TypeCode = CodeType.DimensionType(DimensionTypes.LBP),
                ValueMeasure = new MeasureType
                {
                    unitCode = nameof(FluxUnits.MTR),
                    Value = value
                }
            };
        }

        public static VesselDimensionType GrossTonnage(decimal value)
        {
            return new VesselDimensionType
            {
                TypeCode = CodeType.DimensionType(DimensionTypes.GT),
                ValueMeasure = new MeasureType
                {
                    unitCode = nameof(FluxUnits.TNE),
                    Value = value
                }
            };
        }

        public static VesselDimensionType Breadth(decimal value)
        {
            return new VesselDimensionType
            {
                TypeCode = CodeType.DimensionType(DimensionTypes.BREADTH),
                ValueMeasure = new MeasureType
                {
                    unitCode = nameof(FluxUnits.MTR),
                    Value = value
                }
            };
        }

        public static VesselDimensionType OtherGrossTonage(decimal value)
        {
            return new VesselDimensionType
            {
                TypeCode = CodeType.DimensionType(DimensionTypes.TOTH),
                ValueMeasure = new MeasureType
                {
                    unitCode = nameof(FluxUnits.TNE),
                    Value = value
                }
            };
        }

        public static VesselDimensionType NetTonnage(decimal value)
        {
            return new VesselDimensionType
            {
                TypeCode = CodeType.DimensionType(DimensionTypes.NT),
                ValueMeasure = new MeasureType
                {
                    unitCode = nameof(FluxUnits.TNE),
                    Value = value
                }
            };
        }

        public static VesselDimensionType Draught(decimal value)
        {
            return new VesselDimensionType
            {
                TypeCode = CodeType.DimensionType(DimensionTypes.DRAUGHT),
                ValueMeasure = new MeasureType
                {
                    unitCode = nameof(FluxUnits.TNE),
                    Value = value
                }
            };
        }

        public static VesselDimensionType GetDimension(DimensionTypes type, decimal value, FluxUnits unit = FluxUnits.MTR)
        {
            return new VesselDimensionType
            {
                TypeCode = CodeType.DimensionType(type),
                ValueMeasure = new MeasureType
                {
                    unitCode = unit.ToString(),
                    Value = value
                }
            };
        }

    }
}
