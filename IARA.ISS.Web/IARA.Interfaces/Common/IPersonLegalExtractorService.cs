using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.Interfaces.Common
{
    public interface IPersonLegalExtractorService : IService
    {
        PersonFullDataDTO TryGetPerson(IdentifierTypeEnum identifierType, string identifier);

        LegalFullDataDTO TryGetLegal(string eik);
    }
}
