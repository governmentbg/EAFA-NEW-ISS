namespace IARA.FluxModels
{
    public static class VesselReportTypes
    {
        /// <summary>
        /// Full Submission
        /// </summary>
        public const string SUB = "SUB";

        /// <summary>
        /// Submission of vessel core data
        /// </summary>
        public const string SUB_VCD = "SUB-VCD";

        /// <summary>
        /// Submission of vessel extended data
        /// </summary>
        public const string SUB_VED = "SUB-VED";

        /// <summary>
        /// Submission of a full snapshot
        /// </summary>
        public const string SNAP_F = "SNAP-F";

        /// <summary>
        /// Submission of a partial (limited) snapshot
        /// </summary>
        public const string SNAP_L = "SNAP-L";

        /// <summary>
        /// Submission of a snapshot by upload
        /// </summary>
        public const string SNAP_U = "SNAP-U";
    }
}
