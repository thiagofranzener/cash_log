using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic

namespace CashLogUtils.Extensions
{
    public static class Extensions
    {
        public static double ValNumber(this string value)
        {
            double result;
            try
            {
                result = double.Parse(value);
            }
            catch (Exception ex)
            {
                return 0d;
            }

            return result;
        }

        public static decimal ValString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Conversions.ToDecimal("");
            return Conversions.ToDecimal(value);
        }

        public static bool IsNumber(string value)
        {
            try
            {
                int.Parse(value);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public static bool ObjectSerializationIsInvalid(object value, List<string> errors)
        {
            if (value is null)
            {
                errors.Add("Erro ao serializar objeto");
                return true;
            }

            return false;
        }

        public static bool IsDateValid(DateTime date)
        {
            if (date == default)
                return false;
            if (date < new DateTime(1900, 1, 1))
                return false;
            if (date > new DateTime(2400, 1, 1))
                return false;
            return true;
        }
    }
}