using IARA.Mobile.Domain.Interfaces;
using SQLite;
using System;

namespace IARA.Mobile.Insp.Domain.Entities.Inspections
{
    public class LogBook : IEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int ShipUid { get; set; }
        public string Number { get; set; }
        public DateTime IssuedOn { get; set; }
        public long StartPage { get; set; }
        public long EndPage { get; set; }
    }
}
