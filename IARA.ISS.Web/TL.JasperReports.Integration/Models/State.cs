using System.Collections.Generic;

namespace TL.JasperReports.Integration.Models
{
    public class State
    {
        public string Uri { get; set; }
        public string Id { get; set; }
        public object Value { get; set; }
        public List<Option> Options { get; set; }
    }


}
