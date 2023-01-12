using System;
using System.ComponentModel.DataAnnotations;
using IARA.Mobile.Application.DTObjects.Nomenclatures;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SelectMaxLengthAttribute : ValidationAttribute
    {
        public enum CheckForEnum
        {
            Code,
            Name
        }

        public SelectMaxLengthAttribute(int maxLength, CheckForEnum checkFor = CheckForEnum.Name)
        {
            MaxLength = maxLength;
            CheckFor = checkFor;
        }

        public int MaxLength { get; }
        public CheckForEnum CheckFor { get; }

        public override bool IsValid(object value)
        {
            if (!(value is SelectNomenclatureDto nomenclature))
            {
                return true;
            }

            switch (CheckFor)
            {
                case CheckForEnum.Code:
                    return nomenclature.Code == null
                        || nomenclature.Code.Length < MaxLength;
                case CheckForEnum.Name:
                    return nomenclature.Name == null
                        || nomenclature.Name.Length < MaxLength;
                default:
                    return true;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessage, name, MaxLength);
        }
    }
}
