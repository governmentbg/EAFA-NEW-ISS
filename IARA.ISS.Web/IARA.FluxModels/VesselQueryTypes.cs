namespace IARA.FluxModels
{
    public class VesselQueryTypes
    {
        /// <summary>
        /// Normal query (from MS)
        /// </summary>
        public const string Q_NR = "Q-NR";

        /// <summary>
        /// Query for full snapshot (from COM)
        /// </summary>
        public const string Q_SNAP_F = "Q-SNAP-F";

        /// <summary>
        /// Query for limited snapshot (from COM)
        /// </summary>
        public const string Q_SNAP_L = "Q-SNAP-L";

        /// <summary>
        /// Submission data resulting from a query
        /// </summary>
        public const string SUB_Q = "SUB-Q";
    }
}
