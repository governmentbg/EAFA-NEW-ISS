using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Insp.Domain.Entities.Inspections
{
    public class RecentInspector : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int HistoryId { get; set; }
        public int InspectorId { get; set; }
    }
}
