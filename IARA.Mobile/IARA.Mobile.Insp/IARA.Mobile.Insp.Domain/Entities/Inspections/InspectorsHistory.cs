using System;
using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Insp.Domain.Entities.Inspections
{
    public class InspectorsHistory : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Inspectors { get; set; }
        public int TimesUsed { get; set; }
        public DateTime LastUsed { get; set; }
    }
}
