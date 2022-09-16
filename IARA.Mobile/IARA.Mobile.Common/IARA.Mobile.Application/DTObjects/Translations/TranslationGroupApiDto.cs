using System.Collections.Generic;

namespace IARA.Mobile.Application.DTObjects.Translations
{
    public class TranslationGroupApiDto
    {
        public string Code { get; set; }
        public List<TranslationApiDto> Translations { get; set; }
    }
}
