namespace IARA.Common.Enums
{
    public enum InspDeliveryConfirmationTypesEnum
    {
        /// <summary>
        /// Успешно връчен АУАН
        /// </summary>
        Successful,

        /// <summary>
        /// Нарушителят не се е явил в офис на ИАРА за връчване и получаване на копие от АУАН
        /// </summary>
        NotAppear,

        /// <summary>
        /// Нарушителят отказва да му бъде връчен АУАН
        /// </summary>
        Refusal,

        /// <summary>
        /// Успешно връчено НП
        /// </summary>
        SuccessfulDecree,

        /// <summary>
        /// Нарушителят не се е явил в офис на ИАРА за връчване и получаване на копие от НП
        /// </summary>
        NotAppearDecree,

        /// <summary>
        /// Нарушителят отказва да му бъде връчено НП
        /// </summary>
        RefusalDecree
    }
}
