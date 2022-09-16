using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Translations;
using IARA.DomainModels.Nomenclatures;
using IARA.EntityModels.Entities;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services
{
    public class TranslationService : Service, ITranslationService
    {
        public TranslationService(IARADbContext dbContext)
            : base(dbContext)
        { }

        public Dictionary<string, Dictionary<string, string>> GetWebResources(ResourceLanguageEnum language)
        {
            var dict = GetResourcesByLanguage(language, ResourceTranslationEnum.WEB)
                     .ToDictionary(x => x.Code, x => x.Translations.ToDictionary(y => y.Code, y => y.Translation));

            return dict;
        }

        public List<TranslationGroupDTO> GetResourcesByLanguage(ResourceLanguageEnum language, ResourceTranslationEnum translationType)
        {
            var query = (
                from translationGroup in this.Db.NtranslationGroups
                join translation in this.Db.NtranslationResources on translationGroup.Id equals translation.TranslationGroupId
                where translationGroup.LanguageCode == language.ToString()
                    && translationGroup.TranslationType == translationType.ToString()
                select new
                {
                    GroupCode = translationGroup.Code,
                    ResourceCode = translation.Code,
                    translation.TranslationValue
                }
            ).ToList();

            return query
                .GroupBy(f => f.GroupCode)
                .Select(f => new TranslationGroupDTO
                {
                    Code = f.Key,
                    Translations = f.Select(s => new TranslationDTO
                    {
                        Code = s.ResourceCode,
                        Translation = s.TranslationValue
                    }).ToList()
                }).ToList();
        }

        public List<TranslationLanguageDTO> GetMobileTranslationsWhenUpdated(ResourceTranslationEnum translationType)
        {
            return Enum.GetValues(typeof(ResourceLanguageEnum))
                .Cast<ResourceLanguageEnum>()
                .Select(f => new TranslationLanguageDTO
                {
                    Language = f,
                    Groups = this.GetResourcesByLanguage(f, translationType)
                })
                .ToList();
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            NtranslationResource translation = this.Db.NtranslationResources.First(f => f.Id == id);

            return new SimpleAuditDTO
            {
                CreatedBy = translation.CreatedBy,
                CreatedOn = translation.CreatedOn,
                UpdatedBy = translation.UpdatedBy,
                UpdatedOn = translation.UpdatedOn
            };
        }
    }
}
