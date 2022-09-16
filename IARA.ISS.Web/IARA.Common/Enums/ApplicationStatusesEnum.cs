namespace IARA.Common.Enums
{
    public enum ApplicationStatusesEnum
    {
        ///<summary>Начално състояние</summary>
        INITIAL,

        ///<summary>Попълване от заявителя</summary>
        FILL_BY_APPL,

        ///<summary>Изтеглено заявление за подпис с КЕП</summary>
        APPL_FOR_SIGN,

        ///<summary>Качено валидно подписано заявление в системата</summary>
        UPLOADED_SIGNED,

        ///<summary>Анулирано заявление</summary>
        CANCELED_APPL,

        ///<summary>Стартирани автоматични проверки на заявлението от външни  регистри</summary>
        EXT_CHK_STARTED,

        ///<summary>Проверка и корекции от служител</summary>
        INSP_CORR_FROM_EMP,

        ///<summary>Необходими корекции от заявителя</summary>
        CORR_BY_USR_NEEDED,

        ///<summary>Изчакване на входиране</summary>
        WAIT_ENTRY,

        ///<summary>Изчакване на плащане от заявителя</summary>
        WAIT_PMT_FROM_USR,

        ///<summary>Анулирано плащане</summary>
        PAYMENT_ANNUL,

        ///<summary>Изчакване на проверка за редовност и основание за издаване на административен акт</summary>
        WAIT_REG_CHKS_ISS_GROUNDS,

        ///<summary>Готовност за попълване на административен акт</summary>
        CAN_FILL_ADM_ACT,

        ///<summary>Попълване на (попълнен) административен акт</summary>
        ADM_ACT_COMPLETION,

        ///<summary>Издаден билет</summary>
        TICKET_ISSUED,

        ///<summary>Потвърден издаден билет</summary>
        CONFIRMED_ISSUED_TICKET,

        ///<summary>Входирано заявление в Eventis</summary>
        ENTERED_ASSIGNED_APPL,

        ///<summary>Попълване на заявление от служител</summary>
        FILL_BY_EMP,

        ///<summary>Изчакване въвеждане на платежни данни от служител</summary>
        WAIT_PAYMENT_DATA,

        /// <summary>Обработка на плащането</summary>
        PAYMENT_PROCESSING
    }
}
