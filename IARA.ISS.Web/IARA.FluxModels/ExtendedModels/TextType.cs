namespace IARA.Flux.Models
{
    public partial class TextType
    {
        public static TextType CreateText(string text, string language = "BGR")
        {
            return new TextType
            {
                Value = text,
                languageID = language
            };
        }

        public static TextType[] CreateMultiText(string text, string language = "BGR")
        {
            return new TextType[] { CreateText(text, language) };
        }

        public static implicit operator TextType(string text)
        {
            return CreateText(text);
        }

        public static implicit operator string(TextType text)
        {
            return text.Value;
        }
    }
}
