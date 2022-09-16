namespace IARA.Common.Enums
{
    public enum InspectedFishingGearEnum
    {
        /// <summary>
        /// Риболовния уред в регистъра съвпада с този намерен на борда на кораба
        /// </summary>
        Y,
        /// <summary>
        /// Риболовния уред не съвпада с този на борда
        /// </summary>
        N,
        /// <summary>
        /// Риболовния уред не е регистриран, но е на борда
        /// </summary>
        I,
        /// <summary>
        /// Риболовния уред е регистриран, но не е на борда
        /// </summary>
        R
    }
}
