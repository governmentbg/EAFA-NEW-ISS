using System.Text.Json.Serialization;

namespace IARA.FVMSModels.ExternalModels
{
    public class ContactPerson
    {
        /// <summary>
        /// Име на физическото лице
        /// </summary>
        [JsonPropertyName("first_ame")]
        public string FirstName { get; set; }

        /// <summary>
        /// Бащино име на физическото лице (може да е NULL)
        /// </summary>
        [JsonPropertyName("middle_name")]
        public string MiddleName { get; set; }

        /// <summary>
        /// Фамилия на физическото лице
        /// </summary>
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        public ContactPerson()
        {
        }

        public ContactPerson(ContactPerson rhs)
        {
            FirstName = rhs.FirstName;
            MiddleName = rhs.MiddleName;
            LastName = rhs.LastName;
        }
    }
}
