using System;
using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Insp.Application.Interfaces.Utilities
{
    public interface INomenclatureDates
    {
        DateTime? this[NomenclatureEnum nomenclature] { get; set; }
    }
}
