using System.Text.Json.Serialization;

namespace IARA.FVMSModels.Common
{
    public class ContactLegal
    {
        /// <summary>
        /// Име на юридическото лице
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        public ContactLegal()
        {
        }

        public ContactLegal(ContactLegal rhs)
        {
            Name = rhs.Name;
        }
    }
}
