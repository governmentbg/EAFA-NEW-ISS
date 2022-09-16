using IARA.Mobile.Domain.Enums;
using System.Collections.Generic;

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
