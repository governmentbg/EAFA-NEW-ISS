namespace IARA.SourceGenerators
{
    internal static class DefaultConstants
    {
        public const string BASE_DB_CONTEXT = "BaseIARADbContext";
        public const string DB_CONTEXT = "IARADbContext";
        public const string EXTENDED_DB_CONTEXT = "IARADbContextExtended";


        public const string ENTITIES_FOLDER = "Entities";
        public const string EXTENDED_ENTITIES_FOLDER = "ExtendedEntities";

        public const string IAUDIT_ENTITY = "IAuditEntity";
        public const string IVALIDITY_ENTITY = "IValidity";
        public const string ISOFTDELETABLE_ENTITY = "ISoftDeletable";
        public const string INOMENCLATURE_ENTITY = "INomenclature";
        public const string ICODED_ENTITY = "ICodeEntity";
        public const string IIDENTITY_ENTITY = "IIdentityRecord";
        public const string FILE_ENTITY = "IFileEntity<{0}>";
        public const string ICANCELLABLE_ENTITY = "ICancellableEntity";
        public const string APPLICATION_REGISTER_ENTITY = "IApplicationRegisterEntity";
        public const string APPLICATION_REGISTER_VALIDITY_ENTITY = "IApplicationRegisterValidityEntity";
        public const string NULLABLE_APPLICATION_REGISTER_ENTITY = "INullableApplicationRegisterEntity";
        public const string NULLABLE_APPLICATION_REGISTER_VALIDITY_ENTITY = "INullableApplicationRegisterValidityEntity";
        public const string CHANGE_OF_CIRCUMSTANCES_ENTITY = "IChangeOfCircumstancesEntity";

        public const string ILOGBOOK_PAGE_DECIMAL_ENTITY = "ILogBookPageDecimalEntity";
        public const string ILOGBOOK_PAGE_STRING_ENTITY = "ILogBookPageStringEntity";

        public const string APPLICATION_ENTITY = "IApplicationEntity";

        public const string ENTITIES_NAMESPACE = "IARA.EntityModels.Entities";

        public const string TABLE_ATTRIBUTE_REGEX = @"Table\(""([a-zA-Z_]+)"", Schema = ""([a-zA-Z_]+)""\)";

        public const string CONSTRUCTOR_PATTERN = @"public\s[a-zA-Z]+\(\)\s+{[a-zA-Z<>=\(\);\s]+}";
        public const string CLASS_PATTERN = @"public\spartial\sclass\s[a-zA-Z]+\s+{";


        public const string EXTENDED_DB_CONTEXT_CLASS = @"using IARA.EntityModels.Entities;
using Microsoft.EntityFrameworkCore;

namespace IARA.DataAccess
    {
        public partial class IARADbContext
        {";

        public const string FLUX_NOMENCLATURE_BASE = "FluxNomenclature";
    }
}
