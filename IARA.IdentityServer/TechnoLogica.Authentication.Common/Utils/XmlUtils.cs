using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TechnoLogica.Authentication.Common
{

    public class Utf8StringWriter : StringWriter
    {
        public Utf8StringWriter(StringBuilder builder) : base(builder) { }

        public override Encoding Encoding => Encoding.UTF8;
    }

    public class XmlUtils
    {
        public static string SerializeToXml<T>(T model, XmlSerializerNamespaces ns = null)
        {
            XmlSerializer serializer = new(typeof(T));
            StringBuilder builder = new();

            using (Utf8StringWriter writer = new(builder))
            {
                serializer.Serialize(writer, model, ns);
            }

            string result = builder.ToString();
            return result;
        }

        public static T DeserializeXml<T>(string xml) where T : class
        {
            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }

            XmlSerializer serializer = new(typeof(T));
            using (StringReader reader = new(xml))
            {
                T element = (T)serializer.Deserialize(reader);
                return element;
            }
        }

        public static XmlElement ToXmlElement(string input, bool preserveWhitespaces = false, bool removeXmlTag = true)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            XmlDocument document = new()
            {
                PreserveWhitespace = preserveWhitespaces
            };
            document.LoadXml(input);

            if (removeXmlTag && document.ChildNodes.Count > 1)
            {
                XmlElement element = document.ChildNodes[1] as XmlElement;
                return element;
            }

            return document.DocumentElement;
        }

        // Used in views
        public static string Beautify(string xml, int indentCharsCount = 4)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return xml;
            }

            try
            {
                string newLine = Environment.NewLine;
                XmlDocument doc = new();
                doc.LoadXml(xml);

                StringBuilder sb = new();
                XmlWriterSettings settings = new()
                {
                    Indent = true,
                    IndentChars = new string(' ', indentCharsCount),
                    NewLineChars = newLine,
                    NewLineHandling = NewLineHandling.Replace,
                    OmitXmlDeclaration = true
                };

                using (XmlWriter writer = XmlWriter.Create(sb, settings))
                {
                    doc.Save(writer);
                }

                if (doc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                {
                    // If contains xml tag, add it to result
                    _ = sb.Insert(0, doc.FirstChild.OuterXml + newLine);
                }

                string result = sb.ToString();
                return result;
            }
            catch (Exception)
            {
                return xml;
            }
        }

        public static bool IsValidXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return false;
            }

            try
            {
                XmlDocument document = new();
                document.LoadXml(xml);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
