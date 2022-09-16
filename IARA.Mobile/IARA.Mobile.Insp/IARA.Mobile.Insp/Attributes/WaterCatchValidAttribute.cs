using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class WaterCatchValidAttribute : ValidationAttribute
    {
        private WaterCatchModel parent;

        public void AssignParent(WaterCatchModel parent)
        {
            this.parent = parent;
        }

        public override bool IsValid(object value)
        {
            return IsCatchValid(parent.Dto);
        }

        public override string FormatErrorMessage(string name)
        {
            return null;
        }

        public static bool IsCatchValid(InspectionCatchMeasureDto dto)
        {
            if (dto == null)
            {
                return true;
            }

            if (dto.CatchQuantity == null)
            {
                return false;
            }

            if (dto.Action == null)
            {
                return false;
            }

            if (dto.StorageLocation != null && dto.StorageLocation.Length > 500)
            {
                return false;
            }

            return true;
        }
    }
}
