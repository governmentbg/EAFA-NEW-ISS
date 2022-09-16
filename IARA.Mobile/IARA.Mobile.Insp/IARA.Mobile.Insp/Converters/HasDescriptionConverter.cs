using IARA.Mobile.Application.DTObjects.Nomenclatures;
using System;
using System.Collections;
using System.Globalization;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Converters
{
    public class HasDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DescrSelectNomenclatureDto nom)
            {
                return ConvertFrom(nom);
            }
            else if (value is IList list)
            {
                return ConvertFrom(list);
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static bool ConvertFrom(DescrSelectNomenclatureDto value)
        {
            return value.HasDescription;
        }

        public static bool ConvertFrom(IList values)
        {
            foreach (object value in values)
            {
                if (value is DescrSelectNomenclatureDto nom && nom.HasDescription)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
