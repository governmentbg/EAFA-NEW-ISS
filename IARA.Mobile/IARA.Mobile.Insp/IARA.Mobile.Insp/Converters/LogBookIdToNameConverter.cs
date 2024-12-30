using System;
using System.Collections;
using System.Globalization;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Application.Transactions;
using IARA.Mobile.Insp.Domain.Enums;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Converters
{
    public class LogBookIdToNameConverter : IValueConverter
    {
        public static IInspectionsTransaction InspectionsTransactions { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int logBookId)
            {
                if (InspectionsTransactions == null)
                {
                    InspectionsTransactions = DependencyService.Resolve<IInspectionsTransaction>();
                }
                return InspectionsTransactions.GetLogBookName(logBookId);
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
