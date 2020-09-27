using System;

namespace CashLogUtils.Validators
{
    public partial class Validators
    {
        public Validators()
        {
        }

        public bool ValidateCpf(string value)
        {
            decimal argresult = default;
            if (value is null || value.Length != 11 || !decimal.TryParse(value, out argresult))
                return false;
            return true;
        }

        public bool ValidateEmail(string value)
        {
            if (value is null || !value.Contains("@") || !value.Contains("."))
                return false;
            return true;
        }

        public bool ValidateCnpj(string value)
        {
            decimal argresult = default;
            if (value is null || value.Length != 14 || !decimal.TryParse(value, out argresult))
                return false;
            return true;
        }

        public bool ValidateCep(string value)
        {
            Int32 result;

            if (value is null || value.Length != 8 || int.TryParse(value, out result))
                return false;
            return true;
        }
    }
}
