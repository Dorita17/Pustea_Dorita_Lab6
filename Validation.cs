using System.Globalization;
using System.Windows.Controls;

namespace Pustea_Dorita_Lab6
{
    public class StringNotEmpty : ValidationRule
    {
        //mostenim din clasa ValidationRule
        //suprascriem metoda Validate ce returneaza un ValidationResult
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string aString = value.ToString();

            if (aString == "")
                return new ValidationResult(false, "String cannot be empty");
            return new ValidationResult(true, null);
        }
    }

    public class StringMinLengthValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string aString = value.ToString();

            if (aString.Length < 3)
                return new ValidationResult(false, "String must have at least 3 characters!");
            return new ValidationResult(true, null);
        }
    }
}
