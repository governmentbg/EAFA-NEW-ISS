using System;

namespace IARA.Mobile.Pub.Application.DTObjects.CatchRecords
{
    public class CaughtFishesDto
    {
        public int Id { get; set; }
        public string WaterArea { get; set; }
        public int TotalCount { get; set; }
        public double TotalQuantity { get; set; }
        public DateTime Date { get; set; }
    }
}
