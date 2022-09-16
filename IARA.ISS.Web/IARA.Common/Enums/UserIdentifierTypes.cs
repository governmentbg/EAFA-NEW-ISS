namespace IARA.Common.Enums
{
    public enum UserIdentifierTypes
    {
        /// <summary>
        /// Неразпознат тип идентификатор
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// Единен граждански номер
        /// </summary>
        EGN,

        /// <summary>
        /// Личен номер на чужденец
        /// </summary>
        LNCH,

        /// <summary>
        /// Номер на лична карта
        /// </summary>
        NPID,

        /// <summary>
        /// Номер на паспорт
        /// </summary>
        PASSID,

        /// <summary>
        /// Номер на физическо лице издаден от B-Trust
        /// </summary>
        BTRUST_PID,

        /// <summary>
        /// ДДС номер на физическо лице
        /// </summary>
        VAT_PERSON_ID,

        /// <summary>
        /// ДДС номер на юридическо лице
        /// </summary>
        VAT_LEGAL_ID
    }
}
