using System.Collections.Generic;

namespace TL.JasperReports.Integration.Models
{
    public class InputControl
    {
        public string Id { get; set; }
        public string Lbel { get; set; }
        public string Mandatory { get; set; }
        public string ReadOnly { get; set; }
        public string Type { get; set; }
        public string Uri { get; set; }
        public string Visible { get; set; }
        public Dependencies MasterDependencies { get; set; }
        public Dependencies SlaveDependencies { get; set; }
        public List<ValidationRule> ValidationRules { get; set; }
        public DataType DataType { get; set; }
        public State State { get; set; }
    }


}
