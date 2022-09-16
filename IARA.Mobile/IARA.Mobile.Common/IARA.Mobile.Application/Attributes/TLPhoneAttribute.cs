using System;
using System.ComponentModel.DataAnnotations;

namespace IARA.Mobile.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class TLPhoneAttribute : ValidationAttribute
    {
        private const string _additionalPhoneNumberCharacters = "-.()";

        public override bool IsValid(object value)
        {
            // Automatically pass if value is null or empty. RequiredAttribute should be used to assert a value is not empty.
            string valueAsString = value as string;

            if (string.IsNullOrEmpty(valueAsString))
            {
                return true;
            }

            valueAsString = RemoveExtension(valueAsString.Replace("+", string.Empty).TrimEnd());

            bool digitFound = false;
            foreach (char c in valueAsString)
            {
                if (char.IsDigit(c))
                {
                    digitFound = true;
                    break;
                }
            }

            if (!digitFound)
            {
                return false;
            }

            foreach (char c in valueAsString)
            {
                if (!(char.IsDigit(c)
                    || char.IsWhiteSpace(c)
                    || _additionalPhoneNumberCharacters.IndexOf(c) != -1))
                {
                    return false;
                }
            }
            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessage, name);
        }

        private static string RemoveExtension(string potentialPhoneNumber)
        {
            int lastIndexOfExtension = potentialPhoneNumber
                .LastIndexOf("ext.", StringComparison.InvariantCultureIgnoreCase);
            if (lastIndexOfExtension >= 0)
            {
                string extension = potentialPhoneNumber.Substring(lastIndexOfExtension + 4);
                if (MatchesExtension(extension))
                {
                    return potentialPhoneNumber.Substring(0, lastIndexOfExtension);
                }
            }

            lastIndexOfExtension = potentialPhoneNumber
                .LastIndexOf("ext", StringComparison.InvariantCultureIgnoreCase);
            if (lastIndexOfExtension >= 0)
            {
                string extension = potentialPhoneNumber.Substring(lastIndexOfExtension + 3);
                if (MatchesExtension(extension))
                {
                    return potentialPhoneNumber.Substring(0, lastIndexOfExtension);
                }
            }

            lastIndexOfExtension = potentialPhoneNumber
                .LastIndexOf("x", StringComparison.InvariantCultureIgnoreCase);
            if (lastIndexOfExtension >= 0)
            {
                string extension = potentialPhoneNumber.Substring(lastIndexOfExtension + 1);
                if (MatchesExtension(extension))
                {
                    return potentialPhoneNumber.Substring(0, lastIndexOfExtension);
                }
            }

            return potentialPhoneNumber;
        }

        private static bool MatchesExtension(string potentialExtension)
        {
            potentialExtension = potentialExtension.TrimStart();
            if (potentialExtension.Length == 0)
            {
                return false;
            }

            foreach (char c in potentialExtension)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
