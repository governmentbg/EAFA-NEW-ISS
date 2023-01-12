using System;
using IARA.Mobile.Pub.Domain.Enums;

namespace IARA.Mobile.Pub.Application.Interfaces.Utilities
{
    public interface INomenclatureDates
    {
        DateTime? this[NomenclatureEnum nomenclature] { get; set; }
    }
}
