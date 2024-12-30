using System;
using System.Collections;
using System.Globalization;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Converters
{
    public class IsNotInspectionIGMConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is InspectionType inspection)
            {
                return inspection != InspectionType.IGM;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
