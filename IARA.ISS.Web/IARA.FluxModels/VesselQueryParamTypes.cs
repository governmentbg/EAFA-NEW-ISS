namespace IARA.FluxModels
{
    public class VesselQueryParamTypes
    {
        /// <summary>
        /// Core data only
        /// </summary>
        public const string DATA_VCD = "DATA_VCD";

        /// <summary>
        /// All data (VCD/VED) available
        /// </summary>
        public const string DATA_ALL = "DATA_ALL";

        /// <summary>
        /// Vessel active in the period requested
        /// </summary>
        public const string VESSEL_ACTIVE = "VESSEL_ACTIVE";

        /// <summary>
        /// Any vessel in the period requested
        /// </summary>
        public const string VESSEL_ALL = "VESSEL_ALL";

        /// <summary>
        /// All info about the history of the vessel in the period requested
        /// </summary>
        public const string HIST_YES = "HIST_YES";

        /// <summary>
        /// Last info on the vessel in the period requested
        /// </summary>
        public const string HIST_NO = "HIST_NO";
    }
}
