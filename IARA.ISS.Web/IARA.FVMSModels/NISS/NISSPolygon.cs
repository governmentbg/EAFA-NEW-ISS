using NetTopologySuite.Geometries;

namespace IARA.FVMSModels.NISS
{
    public class NISSPolygon
    {
        public string Identifier { get; set; }
        public string Description { get; set; }
        public int? AltitudeMode { get; set; } = -1;
        public int? Tessellate { get; set; } = -1;
        public int? Extrude { get; set; } = -1;
        public int? Draworder { get; set; } = -1;
        public int? Visibility { get; set; } = -1;
        public Geometry WkbGeometry { get; set; }
    }
}
