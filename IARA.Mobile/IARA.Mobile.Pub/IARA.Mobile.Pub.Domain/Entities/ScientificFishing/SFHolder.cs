using SQLite;

namespace IARA.Mobile.Pub.Domain.Entities.ScientificFishing
{
    public class SFHolder
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public int RequestNumber { get; set; }
        public string Name { get; set; }
        public string ScientificPosition { get; set; }
    }
}
