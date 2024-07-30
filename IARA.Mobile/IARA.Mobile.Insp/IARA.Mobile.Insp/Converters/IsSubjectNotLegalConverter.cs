using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;
using TechnoLogica.Xamarin.Converters.Base;

namespace IARA.Mobile.Insp.Converters
{
    public class IsSubjectNotLegalConverter : BaseValueConverter<bool, SelectNomenclatureDto>
    {
        public override bool ConvertTo(SelectNomenclatureDto value)
        {
            return value.Code != nameof(SubjectType.Legal);
        }
    }
}
