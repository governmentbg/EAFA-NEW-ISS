using System.Collections.Generic;
using System.Text.Json.Serialization;
using IARA.FVMSModels.Common;

namespace IARA.FVMSModels.ExternalModels
{
    public class ContactParty
    {
        /// <summary>
        /// Физическо лице - NULL, ако Legal е налично
        /// </summary>
        [JsonPropertyName("person")]
        public ContactPerson Person { get; set; }

        /// <summary>
        /// Юридическо лице - NULL, ако Person е налично
        /// </summary>
        [JsonPropertyName("legal")]
        public ContactLegal Legal { get; set; }

        /// <summary>
        /// Адрес
        /// </summary>
        [JsonPropertyName("address")]
        public StructuredAddress Address { get; set; }

        /// <summary>
        /// Имейл адреси на ФЛ/ЮЛ
        /// </summary>
        [JsonPropertyName("emails")]
        public List<string> Emails { get; set; }

        /// <summary>
        /// Телефонни номера на ФЛ/ЮЛ
        /// </summary>
        [JsonPropertyName("phone_numbers")]
        public List<string> PhoneNumbers { get; set; }

        public ContactParty()
        {
        }

        public ContactParty(ContactParty rhs)
        {
            Person = rhs.Person != null ? new ContactPerson(rhs.Person) : null;
            Legal = rhs.Legal != null ? new ContactLegal(rhs.Legal) : null;
            Address = rhs.Address != null ? new StructuredAddress(rhs.Address) : null;
            Emails = rhs.Emails != null ? new List<string>(rhs.Emails) : null;
            PhoneNumbers = rhs.PhoneNumbers != null ? new List<string>(rhs.PhoneNumbers) : null;
        }
    }
}
