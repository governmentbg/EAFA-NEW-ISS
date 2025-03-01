﻿using IARA.Mobile.Insp.Domain.Enums;
using TechnoLogica.Xamarin.Converters.Base;

namespace IARA.Mobile.Insp.Converters
{
    public class IsSubjectAquacultureConverter : BaseValueConverter<bool, DeclarationLogBookType>
    {
        public override bool ConvertTo(DeclarationLogBookType value)
        {
            return value == DeclarationLogBookType.AquacultureLogBook;
        }
    }
}
