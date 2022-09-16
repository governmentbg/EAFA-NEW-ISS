using IARA.Mobile.Domain.Enums;
using System.Collections.Generic;

namespace IARA.Mobile.Application.DTObjects.Translations
{
    public class TranslationLanguageApiDto
    {
        public ResourceLanguageEnum Language { get; set; }
        public List<TranslationGroupApiDto> Groups { get; set; }
    }
}
