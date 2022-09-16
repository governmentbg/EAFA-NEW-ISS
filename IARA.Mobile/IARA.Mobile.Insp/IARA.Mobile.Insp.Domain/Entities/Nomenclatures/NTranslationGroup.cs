using IARA.Mobile.Domain.Enums;
using SQLite;

namespace IARA.Mobile.Insp.Domain.Entities.Nomenclatures
{
    public class NTranslationGroup
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public ResourceLanguageEnum Language { get; set; }
        public string Page { get; set; }
    }
}
