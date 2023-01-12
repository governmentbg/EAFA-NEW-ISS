using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Mobile.Application.Interfaces.Transactions;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.Interfaces.Database;
using IARA.Mobile.Insp.Application.Transactions.Base;

namespace IARA.Mobile.Insp.Application.Transactions
{
    public class TranslationTransaction : BaseTransaction, ITranslationTransaction
    {
        public TranslationTransaction(BaseTransactionProvider provider) : base(provider)
        {
        }

        public IReadOnlyDictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>> GetPagesTranslations(IEnumerable<GroupResourceEnum> pages)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                List<string> strPages = pages.Select(s => s.ToString()).ToList();

                return (
                    from translationGroup in context.NTranslationGroups
                    join translation in context.NTranslationResources on translationGroup.Id equals translation.GroupId
                    where strPages.Contains(translationGroup.Page)
                    select new
                    {
                        translation.Code,
                        translation.Value,
                        translationGroup.Page
                    }).ToList()
                    .GroupBy(f => f.Page, f => new { f.Code, f.Value })
                    .ToDictionary(f => Enum.TryParse(f.Key, out GroupResourceEnum page) ? page : default,
                        f => f.ToDictionary(s => s.Code, s => s.Value) as IReadOnlyDictionary<string, string>);
            }
        }
    }
}
