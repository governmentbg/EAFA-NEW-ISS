using IARA.Mobile.Domain.Enums;
using TechnoLogica.Xamarin.Converters.Base;
using TechnoLogica.Xamarin.ResourceTranslator;

namespace IARA.Mobile.Shared.Converters
{
    public class TranslateProfileStatusConverter : BaseValueConverter<string, string>
    {
        public override string ConvertTo(string value)
        {
            string res = TranslateExtension.Translator[nameof(GroupResourceEnum.Profile) + "/Status" + value];

            return string.IsNullOrEmpty(res) ? value : res;
        }
    }
}
