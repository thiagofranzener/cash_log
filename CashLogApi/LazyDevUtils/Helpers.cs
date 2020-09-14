using System;
using System.Collections.Generic;

namespace CashLogUtils
{
    class Helpers
    {

        public double ValNumber(string value)
        {
            double result;
            try
            {
                result = Double.Parse(value);
            } catch 
            {
                result = 0;
            }
            return result;
        }

        public bool IsNumber(string value)
        {
            try
            {
                Double.Parse(value);
            }
            catch 
            {
                return false;
            }
            return true;
        }

        public bool ObjectSerializationIsInvalid(Object value, List<string> errors)
        {
            if (value == null)
            {
                errors.Add("Erro ao serializar objeto");
                return true;
            }
            return false;
        }

        public bool IsDateValid(DateTime data)
        {
            if (data == null) return false;
            if (data < new DateTime(1900, 1, 1)) return false;
            if (data > new DateTime(2400, 1, 1)) return false;
            return true;
        }

        public string EncryptString(string value)
        {
            string result = "";
            System.Security.Cryptography.MD5CryptoServiceProvider cryptoService = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bytesToHash = System.Text.Encoding.ASCII.GetBytes(value);
            foreach (byte streamByte in bytesToHash)
            {
                result += streamByte.ToString("N2");
            }
            return result.ToUpper();
        }

    }
}
