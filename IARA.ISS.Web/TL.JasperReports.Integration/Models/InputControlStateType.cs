using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TL.JasperReports.Integration.Models
{
    public class InputControlStateType
    {
        [JsonPropertyName("inputControlState")]
        public List<InputControlState> InputControlState { get; set; }
    }


}
