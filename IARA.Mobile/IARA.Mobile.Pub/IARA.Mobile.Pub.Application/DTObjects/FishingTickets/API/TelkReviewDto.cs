using System;

namespace IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API
{
    public class TelkReviewDto
    {
        public bool? IsIndefinite { get; set; }
        public string Num { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}
