using IARA.Mobile.Insp.Domain.Enums;
using TechnoLogica.Xamarin.Converters.Base;

namespace IARA.Mobile.Insp.Converters
{
    public class IsSubjectInvoiceConverter : BaseValueConverter<bool, DeclarationLogBookType>
    {
        public override bool ConvertTo(DeclarationLogBookType value)
        {
            return value == DeclarationLogBookType.Invoice;
        }
    }
}
