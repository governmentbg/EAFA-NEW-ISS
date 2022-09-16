using IARA.Mobile.Domain.Enums;

namespace IARA.Mobile.Domain.Interfaces
{
    public interface IDtoResult
    {
        int Id { get; set; }
        DtoResultEnum Result { get; set; }
    }
}
