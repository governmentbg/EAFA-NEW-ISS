using IARA.Mobile.Application.Interfaces.Utilities;
using System;

namespace IARA.Mobile.Infrastructure.Utilities
{
    public class DateTimeUtility : IDateTime
    {
        public DateTime Now => DateTime.Now;
        public DateTime Today => DateTime.Today;
    }
}
