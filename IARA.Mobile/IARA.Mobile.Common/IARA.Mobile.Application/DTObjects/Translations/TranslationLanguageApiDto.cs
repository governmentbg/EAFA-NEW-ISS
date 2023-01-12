using System.Collections.Generic;
using IARA.Mobile.Domain.Enums;

namespace IARA.Mobile.Application.DTObjects.Translations
{
    public class TranslationLanguageApiDto
    {
        public ResourceLanguageEnum Language { get; set; }
        public List<TranslationGroupApiDto> Groups { get; set; }
    }
}
