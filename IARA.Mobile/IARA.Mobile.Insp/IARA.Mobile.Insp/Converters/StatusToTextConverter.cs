using IARA.Mobile.Domain.Enums;
using TechnoLogica.Xamarin.Converters.Base;
using TechnoLogica.Xamarin.ResourceTranslator;

namespace IARA.Mobile.Insp.Converters
{
    public class StatusToTextConverter : BaseValueConverter<string, string>
    {
        override public string ConvertTo(string value)
        {
            return TranslateExtension.Translator[GroupResourceEnum.DeclarationCatch + $"/{value}"];
        }
    }
}
