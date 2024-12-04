using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Domain.Entities.Inspections;
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
            if (values[0] is InspectionState state && values[1] is bool createdByCurrentUser && values[2] is DateTime createdOn)
            {
                int lockedHours = DependencyService.Resolve<ISettings>().LockInspectionAfterHours;
                return
                    (state == InspectionState.Submitted || state == InspectionState.Signed) &&
                    createdByCurrentUser &&
                    DependencyService.Resolve<IProfileTransaction>().HasPermission("InspectionsEditRecords") &&
                    // if -1 the current inspection is has the permission edith the inspection after the lockedHours
                    lockedHours == -1 ? true : createdOn > DateTime.Now.AddHours(-lockedHours);
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
