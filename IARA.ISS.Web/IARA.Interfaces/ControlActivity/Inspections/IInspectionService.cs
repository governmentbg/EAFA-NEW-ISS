using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;

namespace IARA.Interfaces.ControlActivity.Inspections
{
    public interface IInspectionService<T>
        where T : InspectionEditDTO, new()
    {
        T GetEntry(int id);
        int SubmitEntry(T itemDTO, InspectionTypesEnum inspectionType, int userId);
    }
}
