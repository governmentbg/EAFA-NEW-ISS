using System.Collections.Generic;

namespace IARA.WebHelpers.Hubs.Stats
{
    public class Category
    {
        public string Name { get; set; }
        public List<Summary> Summaries { get; set; }
    }
}
