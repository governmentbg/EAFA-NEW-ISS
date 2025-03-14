using IARA.DomainModels.DTOModels.Common;

namespace IARA.Interfaces
{
    public interface ISystemPropertiesService
    {
        SystemPropertiesDTO SystemProperties { get; }

        void Refresh();
    }
}
