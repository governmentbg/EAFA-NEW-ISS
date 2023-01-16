using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using IARA.Mobile.Insp.Models;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DuplicateCatchesAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return !((ICollection)value)
                .Cast<DeclarationCatchModel>()
                .GroupBy(f => new
                {
                    FishId = f.Dto.FishTypeId,
                    TypeId = f.Dto.CatchTypeId,
                    ZoneId = f.Dto.CatchZoneId,
                })
                .Any(f => f.Count() > 1);
        }
    }
}
