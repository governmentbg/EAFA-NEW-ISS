using System;
using System.ComponentModel.DataAnnotations;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Models;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FishingGearValidAttribute : ValidationAttribute
    {
        private FishingGearModel parent;

        public void AssignParent(FishingGearModel parent)
        {
            this.parent = parent;
        }

        public override bool IsValid(object value)
        {
            if (parent.CheckedValue == null)
            {
                return false;
            }

            return IsFishingGearValid(parent.Dto);
        }

        public override string FormatErrorMessage(string name)
        {
            return null;
        }

        public static bool IsFishingGearValid(InspectedFishingGearDto dto)
        {
            if (dto == null)
            {
                return true;
            }

            if (dto.PermittedFishingGear == null && dto.InspectedFishingGear == null)
            {
                return false;
            }

            if (dto.InspectedFishingGear == null)
            {
                return dto.CheckInspectedMatchingRegisteredGear != InspectedFishingGearEnum.I;
            }

            if (dto.CheckInspectedMatchingRegisteredGear == null)
            {
                return false;
            }

            if (dto.InspectedFishingGear.Count < 0)
            {
                return false;
            }

            if (dto.InspectedFishingGear.TypeId < 1)
            {
                return false;
            }

            if (dto.InspectedFishingGear.Height != null && dto.InspectedFishingGear.Height < 1)
            {
                return false;
            }

            if (dto.InspectedFishingGear.HookCount != null && dto.InspectedFishingGear.HookCount < 0)
            {
                return false;
            }

            if (dto.InspectedFishingGear.Length != null && dto.InspectedFishingGear.Length < 1)
            {
                return false;
            }

            if (dto.InspectedFishingGear.NetEyeSize != null && dto.InspectedFishingGear.NetEyeSize < 1)
            {
                return false;
            }

            if (dto.InspectedFishingGear.Description != null && dto.InspectedFishingGear.Description.Length > 5000)
            {
                return false;
            }

            return true;
        }
    }
}
