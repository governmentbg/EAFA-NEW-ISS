namespace IARA.FluxModels.Enums
{
    public enum FaCatchTypes
    {
        /// <summary>
        /// Catch on board at the time of the activity
        /// </summary>
        ONBOARD,
        /// <summary>
        /// Catch allocated to quota
        /// </summary>
        ALLOCATED_TO_QUOTA,
        /// <summary>
        /// Catch taken as a sample to estimate the catch composition
        /// </summary>
        SAMPLE,
        /// <summary>
        /// Catch kept in the net at the time of the activity
        /// </summary>
        KEPT_IN_NET,
        /// <summary>
        /// Catch taken on board during the fishing activity
        /// </summary>
        TAKEN_ONBOARD,
        /// <summary>
        /// Catch or marine animals released during the activity 
        /// (catch is released if the net was never closed beyond the point of retrieval as defined in the regional discard plans)
        /// </summary>
        RELEASED,
        /// <summary>
        /// Catch discarded during the activity (catch was on board)
        /// </summary>
        DISCARDED,
        /// <summary>
        /// Discarded catch to which specifically de minimis exemptions apply
        /// </summary>
        DEMINIMIS,
        /// <summary>
        /// Catches unloaded (for declarations) or to be unloaded (for notifications) from the vessel or its gear during the operation 
        /// (eg. landing, transhipment, relocation, discard at sea)
        /// </summary>
        UNLOADED,
        /// <summary>
        /// Catches loaded (for declarations) or to be loaded (for notifications) onto the vessel during the operation (eg. transhipments, relocation).
        /// </summary>
        LOADED,
        /// <summary>
        /// Incidental catches of aquatic animals
        /// </summary>
        BY_CATCH
    }
}
