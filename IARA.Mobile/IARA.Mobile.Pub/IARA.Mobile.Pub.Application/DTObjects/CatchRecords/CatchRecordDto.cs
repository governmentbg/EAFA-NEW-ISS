using System;

namespace IARA.Mobile.Pub.Application.DTObjects.CatchRecords
{
    public class CatchRecordDto
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public string WaterArea { get; set; }
        public DateTime CatchDate { get; set; }
        public int TotalCount { get; set; }
        public double TotalQuantity { get; set; }
        public bool IsLocal { get; set; }
        public bool IsActive { get; set; }
    }
}
