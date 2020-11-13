using System;

namespace CashLogUtils
{
    class Validators
    {
        Helpers helper = new Helpers();
        public bool ValidateCpf(string value)
        {
            if (value == null || value.Length != 11 || !Decimal.TryParse(value, out decimal result)) return false;
            return true;
        }

        public bool ValidateEmail(string value)
        {
            if (value == null || !value.Contains("@") || !value.Contains(".")) return false;
            return true;
        }

        public bool ValidateCnpj(string value)
        {
            if (value == null || value.Length != 14 || !Decimal.TryParse(value, out decimal result)) return false;
            return true;
        }

        public bool ValidateCep(string value)
        {
            if (value == null || value.Length != 8 || !helper.IsNumber(value)) return false;
            return true;
        }

    }
}
