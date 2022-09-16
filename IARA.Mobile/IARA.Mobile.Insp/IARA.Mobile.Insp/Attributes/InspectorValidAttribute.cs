using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class InspectorValidAttribute : ValidationAttribute
    {
        private InspectorModel parent;

        public void AssignParent(InspectorModel parent)
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

        public static bool IsInspectorValid(InspectorDuringInspectionDto dto)
        {
            if (dto == null)
            {
                return true;
            }

            if (dto.IsNotRegistered)
            {
                if (string.IsNullOrWhiteSpace(dto.CardNum) || dto.CardNum.Length > 5)
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(dto.FirstName) || dto.FirstName.Length > 200)
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(dto.LastName) || dto.LastName.Length > 200)
                {
                    return false;
                }

                if (!dto.CitizenshipId.HasValue)
                {
                    return false;
                }

                if (!dto.InstitutionId.HasValue)
                {
                    return false;
                }
            }
            else if (!dto.InspectorId.HasValue)
            {
                return false;
            }

            return true;
        }
    }
}
