using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Shared.Helpers;
using IARA.Mobile.Shared.ResourceTranslator;

namespace IARA.Mobile.Pub.Utilities
{
    public class TranslatorUtility : ITranslator
    {
        public void ClearResources()
        {
            Translator.Current.HardClear();
        }

        public void LoadOfflineResources()
        {
            Translator.Current.LoadOfflineResources();
        }
    }
}
