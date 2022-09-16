namespace IARA.FluxModels.Enums
{
    public enum FaQueryParameterTypeCodes
    {
        /// <summary>
        /// The identifier of the vessel for which information is requested
        /// </summary>
        VESSELID,

        /// <summary>
        /// The identifier of the trip for which information is requested
        /// </summary>
        TRIPID,
        
        /// <summary>
        /// Y to include only the latest version of the report, N - otherwise
        /// </summary>
        CONSOLIDATED
    }
}
