using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class WaterVesselValidAttribute : ValidationAttribute
    {
        private WaterVesselModel parent;

        public void AssignParent(WaterVesselModel parent)
        {
            this.parent = parent;
        }

        public override bool IsValid(object value)
        {
            return IsVesselValid(parent.Dto);
        }

        public override string FormatErrorMessage(string name)
        {
            return null;
        }

        public static bool IsVesselValid(WaterInspectionVesselDto dto)
        {
            if (dto == null)
            {
                return true;
            }

            if (dto.Number == null || dto.Number.Length > 50)
            {
                return false;
            }

            if (dto.Color == null || dto.Color.Length > 50)
            {
                return false;
            }

            if (dto.Width == null || dto.Width.Value < 1)
            {
                return false;
            }

            if (dto.Length == null || dto.Length.Value < 1)
            {
                return false;
            }

            if (!dto.TotalCount.HasValue || dto.TotalCount.Value < 1)
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
