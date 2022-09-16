using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DeclarationCatchValidAttribute : ValidationAttribute
    {
        private DeclarationCatchModel parent;

        public void AssignParent(DeclarationCatchModel parent)
        {
            this.parent = parent;
        }

        public override bool IsValid(object value)
        {
            return IsDeclarationCatchValid(parent.Dto);
        }

        public override string FormatErrorMessage(string name)
        {
            return null;
        }

        public static bool IsDeclarationCatchValid(InspectedDeclarationCatchDto dto)
        {
            if (dto == null)
            {
                return true;
            }

            if (dto.LogBookType == null)
            {
                return false;
            }

            if (dto.FishTypeId == null)
            {
                return false;
            }

            if (dto.CatchQuantity == null)
            {
                return false;
            }

            if (dto.UnloadedQuantity == null)
            {
                return false;
            }

            if (dto.PresentationId == null)
            {
                return false;
            }

            if (dto.CatchZoneId == null)
            {
                return false;
            }

            if (dto.LogBookType == null)
            {
                return false;
            }

            return true;
        }
    }
}
