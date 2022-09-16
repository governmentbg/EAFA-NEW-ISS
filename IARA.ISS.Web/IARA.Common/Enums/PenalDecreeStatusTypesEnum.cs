namespace IARA.Common.Enums
{
    public enum PenalDecreeStatusTypesEnum
    {
        /// <summary>
        /// Обжалвано пред първа инстанция
        /// </summary>
        FirstInstAppealed,

        /// <summary>
        /// Обжалвано пред втора инстанция
        /// </summary>
        SecondInstAppealed,

        /// <summary>
        /// Постановено решение на първа инстанция
        /// </summary>
        FirstInstDecision,

        /// <summary>
        /// Постановено решение на втора инстанция
        /// </summary>
        SecondInstDecision,

        /// <summary>
        /// Влязло в сила
        /// </summary>
        Valid,

        /// <summary>
        /// Частично изменено
        /// </summary>
        PartiallyChanged,

        /// <summary>
        /// Изцяло отменено
        /// </summary>
        Withdrawn,

        /// <summary>
        /// Предадено за принудително събиране
        /// </summary>
        Compulsory,

        /// <summary>
        /// Изплатено в пълен размер
        /// </summary>
        FullyPaid,

        /// <summary>
        /// Изплатено частично
        /// </summary>
        PartiallyPaid,

        /// <summary>
        /// Разсрочено
        /// </summary>
        Rescheduled
    }
}
