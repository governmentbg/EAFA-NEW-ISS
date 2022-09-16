using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Transactions;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Translations;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services
{
    public class TranslationManagementService : Service, ITranslationManagementService
    {
        private const string HELPER_SUFFIX = "-helper";

        public TranslationManagementService(IARADbContext db)
            : base(db)
        { }

        public IQueryable<TranslationManagementDTO> GetAll(TranslationManagementFilters filters, bool helpers)
        {
            IQueryable<TranslationManagementDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetAll(helpers);
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetAllFilter(filters, helpers)
                    : GetAllFreeText(filters.FreeTextSearch, helpers);
            }

            return result;
        }

        public TranslationManagementEditDTO Get(int id)
        {
            TranslationCodes codes = GetTranslationCodes(id);
            Dictionary<LanguageCodesEnum, string> translations = GetTranslationsByCodes(codes);

            TranslationManagementEditDTO result = new TranslationManagementEditDTO
            {
                Id = id,
                Code = codes.ResCode,
                GroupCode = codes.GroupCode
            };

            if (translations.TryGetValue(LanguageCodesEnum.EN, out string valueEn))
            {
                result.ValueEn = valueEn;
            }

            if (translations.TryGetValue(LanguageCodesEnum.BG, out string valueBg))
            {
                result.ValueBg = valueBg;
            }

            return result;
        }

        public TranslationManagementEditDTO GetByKey(string key)
        {
            (string groupCode, string resourceCode) = SplitResourceKey(key);

            int id = (from res in Db.NtranslationResources
                      join grp in Db.NtranslationGroups on res.TranslationGroupId equals grp.Id
                      where grp.Code == groupCode
                            && res.Code == resourceCode
                      select res.Id).FirstOrDefault();

            return id != 0 ? Get(id) : null;
        }

        public int AddEntry(TranslationManagementEditDTO resource)
        {
            Dictionary<LanguageCodesEnum, NtranslationGroup> group = AddGroupIfNotExists(resource.GroupCode);

            NtranslationResource en = BuildEntry(resource.Code, resource.ValueEn, group[LanguageCodesEnum.EN]);
            NtranslationResource bg = BuildEntry(resource.Code, resource.ValueBg, group[LanguageCodesEnum.BG]);

            Db.NtranslationResources.Add(en);
            Db.NtranslationResources.Add(bg);

            OnTranslationsUpdated();
            Db.SaveChanges();

            return en.Id;
        }

        public void EditEntry(TranslationManagementEditDTO resource)
        {
            TranslationCodes codes = GetTranslationCodes(resource.Id.Value);
            Dictionary<LanguageCodesEnum, NtranslationResource> resources = GetTranslationResources(codes);

            resources[LanguageCodesEnum.BG].TranslationValue = resource.ValueBg;
            resources[LanguageCodesEnum.EN].TranslationValue = resource.ValueEn;

            OnTranslationsUpdated();
            Db.SaveChanges();
        }

        public List<NomenclatureDTO> GetGroups()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> result = (from grp in Db.NtranslationGroups
                                            where grp.ValidFrom <= now && grp.ValidTo > now
                                            group grp by grp.Code into grouped
                                            orderby grouped.Key
                                            select new NomenclatureDTO
                                            {
                                                DisplayName = grouped.Key
                                            }).ToList();

            return result;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.NtranslationResources, id);
            return audit;
        }

        private IQueryable<TranslationManagementDTO> GetAll(bool helpers)
        {
            IQueryable<TranslationManagementDTO> result = GetAllBaseQuery(helpers);
            return result;
        }

        private IQueryable<TranslationManagementDTO> GetAllFilter(TranslationManagementFilters filters, bool helpers)
        {
            IQueryable<TranslationManagementDTO> result = GetAllBaseQuery(helpers);

            if (!string.IsNullOrEmpty(filters.Code))
            {
                result = result.Where(x => x.Code.ToLower().Contains(filters.Code.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.GroupCode))
            {
                result = result.Where(x => x.GroupCode.ToLower().Contains(filters.GroupCode.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.TranslationType))
            {
                result = result.Where(x => x.GroupType.ToLower().Contains(filters.TranslationType.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.TranslationValueBG))
            {
                result = result.Where(x => x.ValueBg.ToLower().Contains(filters.TranslationValueBG.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.TranslationValueEN))
            {
                result = result.Where(x => x.ValueEn.ToLower().Contains(filters.TranslationValueEN.ToLower()));
            }

            return result;
        }

        private IQueryable<TranslationManagementDTO> GetAllFreeText(string text, bool helpers)
        {
            text = text.ToLowerInvariant();

            IQueryable<TranslationManagementDTO> result = GetAllBaseQuery(helpers);

            result = from trans in result
                     where trans.Code.ToLower().Contains(text)
                        || trans.GroupCode.ToLower().Contains(text)
                        || trans.GroupType.ToLower().Contains(text)
                        || trans.ValueBg.ToLower().Contains(text)
                        || trans.ValueEn.ToLower().Contains(text)
                     select trans;

            return result;
        }

        private IQueryable<TranslationManagementDTO> GetAllBaseQuery(bool helpers)
        {
            DateTime now = DateTime.Now;

            var result = from res in Db.NtranslationResources
                         join grp in Db.NtranslationGroups on res.TranslationGroupId equals grp.Id
                         where res.ValidFrom <= now
                            && res.ValidTo > now
                            && grp.ValidFrom <= now
                            && grp.ValidTo > now
                            && (res.ResourceType == nameof(TranslationResourceTypeEnum.Help) == helpers)
                         group new { res, grp } by new { ResCode = res.Code, GrpCode = grp.Code, grp.TranslationType } into grouped
                         orderby grouped.Key.GrpCode, grouped.Key.ResCode
                         select new TranslationManagementDTO
                         {
                             Id = grouped.Min(x => x.res.Id),
                             Code = grouped.Key.ResCode,
                             GroupCode = grouped.Key.GrpCode,
                             GroupType = grouped.Key.TranslationType,
                             ValueBg = grouped
                                 .Where(x => x.grp.LanguageCode == nameof(LanguageCodesEnum.BG))
                                 .Max(x => x.res.TranslationValue),
                             ValueEn = grouped
                                .Where(x => x.grp.LanguageCode == nameof(LanguageCodesEnum.EN))
                                .Max(x => x.res.TranslationValue),
                             IsActive = true
                         };

            return result;
        }

        private Dictionary<LanguageCodesEnum, NtranslationResource> GetTranslationResources(TranslationCodes codes)
        {
            var translations = (from res in Db.NtranslationResources
                                join grp in Db.NtranslationGroups on res.TranslationGroupId equals grp.Id
                                where res.Code == codes.ResCode
                                       && grp.Code == codes.GroupCode
                                       && grp.TranslationType == codes.TranslationType
                                select new
                                {
                                    LanguageCode = Enum.Parse<LanguageCodesEnum>(grp.LanguageCode),
                                    Resource = res
                                }).ToDictionary(x => x.LanguageCode, y => y.Resource);

            return translations;
        }

        private Dictionary<LanguageCodesEnum, string> GetTranslationsByCodes(TranslationCodes codes)
        {
            var translations = (from res in Db.NtranslationResources
                                join grp in Db.NtranslationGroups on res.TranslationGroupId equals grp.Id
                                where res.Code == codes.ResCode
                                       && grp.Code == codes.GroupCode
                                       && grp.TranslationType == codes.TranslationType
                                select new
                                {
                                    LanguageCode = Enum.Parse<LanguageCodesEnum>(grp.LanguageCode),
                                    res.TranslationValue
                                }).ToDictionary(x => x.LanguageCode, y => y.TranslationValue);

            return translations;
        }

        private TranslationCodes GetTranslationCodes(int resourceId)
        {
            var codes = (from res in Db.NtranslationResources
                         join grp in Db.NtranslationGroups on res.TranslationGroupId equals grp.Id
                         where res.Id == resourceId
                         select new TranslationCodes
                         {
                             ResCode = res.Code,
                             GroupCode = grp.Code,
                             TranslationType = grp.TranslationType
                         }).First();

            return codes;
        }

        private Dictionary<LanguageCodesEnum, NtranslationGroup> AddGroupIfNotExists(string code)
        {
            var groups = (from grp in Db.NtranslationGroups
                          where grp.Code == code
                          select grp).ToDictionary(x => Enum.Parse<LanguageCodesEnum>(x.LanguageCode));

            if (groups.Count != 2)
            {
                if (!groups.ContainsKey(LanguageCodesEnum.BG))
                {
                    NtranslationGroup entry = new NtranslationGroup
                    {
                        Code = code,
                        Name = code,
                        LanguageCode = nameof(LanguageCodesEnum.BG),
                        TranslationType = nameof(TranslationTypeEnum.WEB)
                    };

                    Db.NtranslationGroups.Add(entry);

                    groups.Add(LanguageCodesEnum.BG, entry);
                }

                if (!groups.ContainsKey(LanguageCodesEnum.EN))
                {
                    NtranslationGroup entry = new NtranslationGroup
                    {
                        Code = code,
                        Name = code,
                        LanguageCode = nameof(LanguageCodesEnum.EN),
                        TranslationType = nameof(TranslationTypeEnum.WEB)
                    };

                    Db.NtranslationGroups.Add(entry);

                    groups.Add(LanguageCodesEnum.EN, entry);
                }
            }

            return groups;
        }

        private static NtranslationResource BuildEntry(string code, string translation, NtranslationGroup group)
        {
            NtranslationResource entry = new NtranslationResource
            {
                TranslationGroup = group,
                Code = code,
                TranslationValue = translation,
                ResourceType = code.EndsWith(HELPER_SUFFIX)
                    ? nameof(TranslationResourceTypeEnum.Help)
                    : nameof(TranslationResourceTypeEnum.Label)
            };

            return entry;
        }

        private void OnTranslationsUpdated()
        {
            string tableName = typeof(NtranslationResource).GetCustomAttribute<TableAttribute>().Name;
            string groupTableName = typeof(NtranslationGroup).GetCustomAttribute<TableAttribute>().Name;

            List<NnomenclatureTable> tables = (from table in Db.NnomenclatureTables
                                               where table.Name == tableName
                                                    || table.Name == groupTableName
                                               select table).ToList();

            foreach (NnomenclatureTable table in tables)
            {
                table.DataLastEditOn = DateTime.Now;
            }
        }

        private static (string group, string resource) SplitResourceKey(string key)
        {
            string[] parts = key.Split('.');
            string groupCode = parts[0];
            string resourceCode = parts[1];

            return (groupCode, resourceCode);
        }
    }

    internal class TranslationCodes
    {
        public string ResCode { get; set; }

        public string GroupCode { get; set; }

        public string TranslationType { get; set; }
    }
}
