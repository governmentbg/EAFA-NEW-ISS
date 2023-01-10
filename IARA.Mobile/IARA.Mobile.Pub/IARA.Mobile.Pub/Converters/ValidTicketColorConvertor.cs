using System;
using System.Globalization;
using IARA.Mobile.Pub.Domain.Enums;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.Converters
{
    public class ValidTicketColorConvertor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (object value in values)
            {
                if (value is null)
                {
                    return Color.Red;
                }
            }

            PaymentStatusEnum paymentStatus = (PaymentStatusEnum)Enum.Parse(typeof(PaymentStatusEnum), values[0].ToString(), true);
            DateTime validTo = System.Convert.ToDateTime(values[1]);
            string applicationStatusCode = values[2].ToString();
            string ticketStatusCode = values[3].ToString();

            bool needCorrections = applicationStatusCode == ApplicationStatuses.CORR_BY_USR_NEEDED || applicationStatusCode == ApplicationStatuses.FILL_BY_APPL; //Нужна е корекция от потребител
            DateTime now = DateTime.Now;

            if (now > validTo || ticketStatusCode == nameof(TicketStatusEnum.EXPIRED))
            {
                return Color.Red;
            }
            else if ((validTo > now && !needCorrections && paymentStatus == PaymentStatusEnum.Unpaid)
                || (applicationStatusCode == ApplicationStatuses.PAYMENT_PROCESSING))
            {
                return Color.Gray;
            }
            else if (validTo > now &&
                   (paymentStatus == PaymentStatusEnum.PaidOK || paymentStatus == PaymentStatusEnum.NotNeeded) &&
                    !needCorrections &&
                    (ticketStatusCode == nameof(TicketStatusEnum.APPROVED) || ticketStatusCode == nameof(TicketStatusEnum.ISSUED) || ticketStatusCode == nameof(TicketStatusEnum.REQUESTED)))
            {
                return Color.Green;
            }

            return Color.Red;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
