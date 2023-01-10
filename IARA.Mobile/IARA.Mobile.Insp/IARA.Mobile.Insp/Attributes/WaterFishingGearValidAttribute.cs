using System;
using System.ComponentModel.DataAnnotations;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Models;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class WaterFishingGearValidAttribute : ValidationAttribute
    {
        private WaterFishingGearModel parent;

        public void AssignParent(WaterFishingGearModel parent)
        {
            this.parent = parent;
        }

        public override bool IsValid(object value)
        {
            return IsFishingGearValid(parent.Dto);
        }

        public override string FormatErrorMessage(string name)
        {
            return null;
        }

        public static bool IsFishingGearValid(WaterInspectionFishingGearDto dto)
        {
            if (dto == null)
            {
                return true;
            }

            if (dto.Count < 0)
            {
                return false;
            }

            if (dto.TypeId < 1)
            {
                return false;
            }

            if (dto.Height == null || dto.Height < 1)
            {
                return false;
            }

            if (dto.HookCount == null || dto.HookCount < 0)
            {
                return false;
            }

            if (dto.Length == null || dto.Length < 1)
            {
                return false;
            }

            if (dto.NetEyeSize == null || dto.NetEyeSize < 1)
            {
                return false;
            }

            if (dto.Description != null && dto.Description.Length > 4000)
            {
                return false;
            }

            return true;
        }
    }
}
