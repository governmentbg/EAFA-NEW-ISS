using IARA.Security.Enums;

namespace IARA.Security
{
    public interface ISecurityEmailSender
    {
        /// <summary>
        /// Добавя email в опашката за изпращане
        /// </summary>
        /// <param name="email">email адрес</param>
        /// <param name="baseAddress">базов адрес на услугата</param>
        /// <param name="emailType">Тип на email който да се изпрати</param>
        /// <returns>Security Token</returns>
        string EnqueuePasswordEmail(string email, SecurityEmailTypes emailType, string baseAddress = null);
    }
}
