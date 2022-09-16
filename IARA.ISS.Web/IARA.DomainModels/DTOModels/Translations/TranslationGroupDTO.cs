using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.Translations
{
    public class TranslationGroupDTO
    {
        public string Code { get; set; }
        public List<TranslationDTO> Translations { get; set; }
    }
}
