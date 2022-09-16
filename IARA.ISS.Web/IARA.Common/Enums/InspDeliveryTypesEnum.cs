namespace IARA.Common.Enums
{
    public enum InspDeliveryTypesEnum
    {
        /// <summary>
        /// Лично на нарушителя
        /// </summary>
        Personal,

        /// <summary>
        /// Нарушителят е поканен да даде обяснения, подпише и получи екземпляр от съставения АУАН в офис на ИАРА
        /// </summary>
        Office,

        /// <summary>
        /// Връчване със съдействието на друг държавен орган
        /// </summary>
        StateService,

        /// <summary>
        /// Връчване чрез общинска служба или администрация по местоживеене
        /// </summary>
        ByMail,

        /// <summary>
        /// Електронно връчване през Система за сигурно електронно връчване
        /// </summary>
        EDelivery,

        /// <summary>
        //Нарушителят отказва да му бъде връчен АУАН
        /// <summary>
        Refusal,

        /// <summary>
        //Нарушителят не се е явил в офис на ИАРА за връчване и получаване на копие от АУАН
        /// <summary>
        NotAppear,

        /// <summary>
        //НП е връчено лично
        /// <summary>
        DecreePersonal,

        /// <summary>
        //НП е изпратено с обратна раписка
        /// <summary>
        DecreeReturn,

        /// <summary>
        //НП е връчено чрез ИС за сигурно електронно връчване
        /// <summary>
        DecreeEDelivery,

        /// <summary>
        //НП е връчено чрез отбелязване
        /// <summary>
        DecreeTag
    }
}
