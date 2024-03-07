using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Shared.ResourceTranslator;

namespace IARA.Mobile.Insp.Utilities
{
    public class TranslatorUtility : ITranslator
    {
        public void ClearResources()
        {
            Translator.Current.SoftClear();
        }
    }
}
