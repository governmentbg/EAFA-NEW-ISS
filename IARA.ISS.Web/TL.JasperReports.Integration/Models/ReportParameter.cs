using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace TL.JasperReports.Integration
{
    [XmlRoot(ElementName = "reportParameter")]
    public class ReportParameter
    {
        public ReportParameter()
        {
            this.Values = new List<string>();
        }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "value")]
        public List<string> Values { get; set; }

        [XmlIgnore]
        public string Value
        {
            get
            {
                return Values.FirstOrDefault();
            }
            set
            {
                if (Values.Count == 0)
                {
                    Values.Add(value);
                }
                else
                {
                    Values[0] = value;
                }
            }
        }
    }
}
