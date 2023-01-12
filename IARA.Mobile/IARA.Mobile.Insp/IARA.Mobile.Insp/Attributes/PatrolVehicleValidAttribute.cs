using System;
using System.ComponentModel.DataAnnotations;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Models;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PatrolVehicleValidAttribute : ValidationAttribute
    {
        private PatrolVehicleModel parent;

        public void AssignParent(PatrolVehicleModel parent)
        {
            this.parent = parent;
        }

        public override bool IsValid(object value)
        {
            return IsInspectorValid(parent.Dto);
        }

        public override string FormatErrorMessage(string name)
        {
            return null;
        }

        public static bool IsInspectorValid(VesselDuringInspectionDto dto)
        {
            if (dto == null)
            {
                return true;
            }

            if (dto.IsRegistered == true)
            {
                return dto.UnregisteredVesselId.HasValue;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Length > 500)
                {
                    return false;
                }

                if (dto.CFR == null || dto.CFR.Length > 20)
                {
                    return false;
                }

                if (dto.ExternalMark == null || dto.ExternalMark.Length > 50)
                {
                    return false;
                }

                if (dto.RegularCallsign == null || dto.RegularCallsign.Length > 50)
                {
                    return false;
                }

                if (!dto.FlagCountryId.HasValue)
                {
                    return false;
                }

                if (!dto.InstitutionId.HasValue)
                {
                    return false;
                }

                if (!dto.PatrolVehicleTypeId.HasValue)
                {
                    return false;
                }

                if (dto.Location == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
