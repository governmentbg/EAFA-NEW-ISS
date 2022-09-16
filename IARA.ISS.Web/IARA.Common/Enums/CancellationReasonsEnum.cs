namespace IARA.Common.Enums
{
    public enum CancellationReasonsEnum
    {
        /// <summary>
        /// По желание на собственика
        /// </summary>
        OwnerRequested,

        /// <summary>
        /// С решение на Изпълнителния директор на ИАРА
        /// </summary>
        IARAHeadDecision,

        /// <summary>
        /// С решение на Министъра на земеделието и храните
        /// </summary>
        MZHHeadDecision,

        /// <summary>
        /// Служебно отписан
        /// </summary>
        ManuallyDeregistered,

        /// <summary>
        ///  Унищожаване
        /// </summary>
        ShipDestroy,

        /// <summary>
        /// Корабокрушение
        /// </summary>
        ShipWreck,

        /// <summary>
        /// Не е дефинирано
        /// </summary>
        Unknown,

        /// <summary>
        ///  Друго
        /// </summary>
        Other
    }
}
