using IARA.Mobile.Application.DTObjects.Nomenclatures;
using System;
using System.Collections;
using System.Globalization;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Converters
{
    public class HasFishingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SelectNomenclatureDto nom)
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

        public static bool ConvertFrom(SelectNomenclatureDto value)
        {
            return value.Code == "Fishing";
        }

        public static bool ConvertFrom(IList values)
        {
            foreach (object value in values)
            {
                if (value is SelectNomenclatureDto nom && nom.Code == "Fishing")
                {
                    return true;
                }
            }

            return false;
        }
    }
}
