using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Application.Interfaces.Utilities
{
    public interface INomenclatureDatesClear
    {
        void Clear();
        void Remove(NomenclatureEnum nomenclatureEnum);
    }
}
