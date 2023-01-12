using System.Collections.Generic;
using IARA.Mobile.Domain.Enums;

namespace IARA.Mobile.Application.Interfaces.Transactions
{
    public interface ITranslationTransaction
    {
        /// <summary>
        /// Gets the saved resources for the given pages from the database
        /// </summary>
        IReadOnlyDictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>> GetPagesTranslations(IEnumerable<GroupResourceEnum> pages);
    }
}
