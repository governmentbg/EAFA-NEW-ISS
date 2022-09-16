using IARA.Mobile.Insp.Domain.Enums;
using System;

namespace IARA.Mobile.Insp.Application.Interfaces.Utilities
{
    public interface INomenclatureDates
    {
        DateTime? this[NomenclatureEnum nomenclature] { get; set; }
    }
}
