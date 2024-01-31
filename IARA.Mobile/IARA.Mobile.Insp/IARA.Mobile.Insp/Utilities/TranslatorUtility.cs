using IARA.Mobile.Application.Interfaces.Transactions;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Shared.ResourceTranslator;
using System.Collections.Generic;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Utilities
{
    public class TranslatorUtility : ITranslator
    {
        public void ClearResources()
        {
            Translator.Current.HardClear();
        }

        public void LoadOfflineResources()
        {
            //GroupResourceEnum[] safeResources = new[] { GroupResourceEnum.Common, GroupResourceEnum.Menu };

            //IReadOnlyDictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>> resources =
            //    DependencyService.Resolve<ITranslationTransaction>()
            //        .GetPagesTranslations(safeResources);

            //Translator.Current.Remove(safeResources);
            //Translator.Current.SafeResources(safeResources);
            //Translator.Current.Add(resources);
        }
    }
}
