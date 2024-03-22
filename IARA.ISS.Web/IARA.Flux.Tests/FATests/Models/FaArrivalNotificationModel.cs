using System;
using IARA.Flux.Models;

namespace IARA.Flux.Tests.FATests.Models
{
    public class FaArrivalNotificationModel
    {
        public DateTime Occurrence { get; set; }

        public string ReasonCode { get; set; }

        public DateTime TripStartDate { get; set; }

        public DateTime TripEndDate { get; set; }

        public FLUXLocationType[] Locations { get; set; }

        public FACatchType[] Catches { get; set; }
    }
}
