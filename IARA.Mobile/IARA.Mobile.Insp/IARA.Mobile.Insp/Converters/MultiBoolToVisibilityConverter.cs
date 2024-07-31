using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Domain.Enums;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Converters
{
    public class MultiBoolToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is InspectionState state && values[1] is bool createdByCurrentUser)
            {
                return (state == InspectionState.Submitted || state == InspectionState.Signed) &&
                    createdByCurrentUser &&
                    DependencyService.Resolve<IProfileTransaction>().HasPermission("InspectionsEditRecords");
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
