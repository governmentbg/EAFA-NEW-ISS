using System;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels
{
    public class NotificationDTO
    {
        public int Id { get; set; }
        public string Icon { get; set; }
        public string Subtitle { get; set; }
        public string Text { get; set; }
        public bool IsRead { get; set; }
        public string Url { get; set; }
        public string Title { get; private set; }

        private DateTime receivedDate;
        public DateTime RecievedDate
        {
            get
            {
                return receivedDate;
            }
            set
            {
                receivedDate = value;
                Title = FormatDate(value);
            }
        }

        private static int GetWeekDay(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)date.DayOfWeek;
        }

        private static string FormatDate(DateTime messageDate)
        {
            DateTime now = DateTime.Now;

            if ((now - messageDate).TotalSeconds < 30)
            {
                return AppResources.msgNow;
            }
            else if (now.Date == messageDate.Date)
            {
                return messageDate.ToString("HH:mm:ss");
            }
            else if ((now.Date - messageDate.Date).TotalDays <= 7 && GetWeekDay(now.Date) > GetWeekDay(messageDate.Date))
            {
                return messageDate.ToString("dddd HH:mm:ss");
            }
            else if (now.Date.Year == messageDate.Date.Year)
            {
                return messageDate.ToString("dd MMMM HH:mm:ss");
            }
            else
            {
                return messageDate.ToString("dd.MM.yyyy HH:mm:ss");
            }
        }
    }
}
