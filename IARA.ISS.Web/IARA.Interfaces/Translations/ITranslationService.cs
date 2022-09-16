using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Translations;

namespace IARA.Interfaces
{
    public interface ITranslationService
    {
        /// <summary>
        /// Get all the resources for the given language
        /// </summary>
        List<TranslationGroupDTO> GetResourcesByLanguage(ResourceLanguageEnum language, ResourceTranslationEnum translationType);

        /// <summary>
        /// Gets all the resources for all languages
        /// </summary>
        List<TranslationLanguageDTO> GetMobileTranslationsWhenUpdated(ResourceTranslationEnum translationType);
        Dictionary<string, Dictionary<string, string>> GetWebResources(ResourceLanguageEnum language);
    }
}
