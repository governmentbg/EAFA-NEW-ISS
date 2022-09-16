using System.Text.Json.Serialization;

namespace IARA.FVMSModels.NISS
{
    public class NISSQuery
    {
        public string Identifier { get; set; }

        [JsonIgnore]
        public NISSQueryType Type { get; set; }
    }
}
