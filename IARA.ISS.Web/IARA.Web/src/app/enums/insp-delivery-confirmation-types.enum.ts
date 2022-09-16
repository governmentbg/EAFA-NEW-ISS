export enum InspDeliveryConfirmationTypesEnum {
    // Успешно връчен АУАН
    Successful,

    // Нарушителят не се е явил в офис на ИАРА за връчване и получаване на копие от АУАН
    NotAppear,

    // Нарушителят отказва да му бъде връчен АУАН
    Refusal,

    // Успешно връчено НП
    SuccessfulDecree,

    // Нарушителят не се е явил в офис на ИАРА за връчване и получаване на копие от НП
    NotAppearDecree,

    // Нарушителят отказва да му бъде връчено НП
    RefusalDecree,
}