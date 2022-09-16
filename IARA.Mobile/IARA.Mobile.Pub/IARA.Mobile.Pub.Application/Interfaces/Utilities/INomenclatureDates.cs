using IARA.Mobile.Pub.Domain.Enums;
using System;

namespace IARA.Mobile.Pub.Application.Interfaces.Utilities
{
    public interface INomenclatureDates
    {
        DateTime? this[NomenclatureEnum nomenclature] { get; set; }
    }
}
