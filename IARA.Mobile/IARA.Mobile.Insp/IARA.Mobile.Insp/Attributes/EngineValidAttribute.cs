using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EngineValidAttribute : ValidationAttribute
    {
        private EngineModel parent;

        public void AssignParent(EngineModel parent)
        {
            this.parent = parent;
        }

        public override bool IsValid(object value)
        {
            return IsEngineValid(parent.Dto);
        }

        public override string FormatErrorMessage(string name)
        {
            return null;
        }

        public static bool IsEngineValid(WaterInspectionEngineDto dto)
        {
            if (dto == null)
            {
                return true;
            }

            if (dto.Model == null || dto.Model.Length > 50)
            {
                return false;
            }

            if (!dto.Power.HasValue || dto.Power.Value < 1 || dto.Power.Value > 999.99m)
            {
                return false;
            }

            if (dto.Type == null || dto.Type.Length > 50)
            {
                return false;
            }

            if (dto.EngineDescription == null || dto.EngineDescription.Length > 500)
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
