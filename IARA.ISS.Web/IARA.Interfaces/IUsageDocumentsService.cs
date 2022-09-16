using System.Collections.Generic;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.Interfaces
{
    public interface IUsageDocumentsService : IService
    {
        UsageDocumentDTO GetUsageDocument(int id);

        List<UsageDocumentDTO> GetUsageDocuments(List<int> ids);

        UsageDocumentRegixDataDTO GetUsageDocumentRegixData(int id);
    }
}
