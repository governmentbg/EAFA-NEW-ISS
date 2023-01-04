using IARA.Mobile.Insp.Domain.Enums;
using TechnoLogica.Xamarin.Converters.Base;

namespace IARA.Mobile.Insp.Converters
{
    public class IsSubjectShipConverter : BaseValueConverter<bool, DeclarationLogBookType>
    {
        public override bool ConvertTo(DeclarationLogBookType value)
        {
            return value == DeclarationLogBookType.ShipLogBook
                || value == DeclarationLogBookType.FirstSaleLogBook
                || value == DeclarationLogBookType.TransportationLogBook
                || value == DeclarationLogBookType.AdmissionLogBook;
        }
    }
}
