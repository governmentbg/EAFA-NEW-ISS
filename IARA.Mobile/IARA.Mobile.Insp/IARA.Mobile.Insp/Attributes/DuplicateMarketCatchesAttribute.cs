using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using IARA.Mobile.Insp.Controls.ViewModels;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DuplicateMarketCatchesAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return !((ICollection)value)
                .Cast<CatchInspectionViewModel>()
                .GroupBy(f => new
                {
                    FishId = f.FishType.Value?.Id,
                    TypeId = f.FishType.Value?.Id,
                    ZoneId = f.CatchArea.Value?.Id,
                    TurbotSizeId = f.TurbotSizeGroup.Value?.Id,
                })
                .Any(f => f.Count() > 1);
        }
    }
}
