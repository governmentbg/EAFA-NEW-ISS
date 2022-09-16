using System.Collections.Generic;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.Translations
{
    public class TranslationLanguageDTO
    {
        public ResourceLanguageEnum Language { get; set; }
        public List<TranslationGroupDTO> Groups { get; set; }
    }
}
