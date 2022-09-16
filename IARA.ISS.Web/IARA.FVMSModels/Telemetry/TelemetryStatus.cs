using IARA.FVMSModels.Stuctures;

namespace IARA.FVMSModels
{
    public class TelemetryStatus
    {
        public string ParameterName { get; set; }
        public string Type { get; set; }
        public double Value { get; set; }
        public Position Position { get; set; }

        /// <summary>
        /// CFR
        /// При запитване за множество РК отговорът съдържа множество структури от тип TelStatus, които трябва да се идентифицират със съответен РК.
        /// </summary>
        public string VesselIdentifier { get; set; }
    }
}
