using System.Collections.Generic;
using IARA.FVMSModels.Common;

namespace IARA.Interfaces.FVMSIntegrations
{
    public interface IFVMSNomenclatureDataService
    {
        List<FvmsFish> GetFishTypes();

        List<FvmsInspector> GetInspectors();
    }
}
