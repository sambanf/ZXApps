using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Asp.NETMVCCRUD.Class
{
    public class StringHelper
    {
        public static string SetEmptyStringIfNull(object obj)
        {
            try
            {
                return obj.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static bool IsNumeric(String strVal)
        {
            Regex reg = new Regex("[^0-9-]");
            Regex reg2 = new Regex("^-[0-9]+$|^[0-9]+$");
            return (!reg.IsMatch(strVal) && reg2.IsMatch(strVal));
        }

        public static string[] SplitWord(string input, params char[] CharSeparator)
        {
            return input.Split(CharSeparator, StringSplitOptions.None);
        }

        public static int WordCount(string input, params char[] CharSeparator)
        {
            return input.Split(CharSeparator, StringSplitOptions.None).Length;
        }

        static public string Left(string str, int Length)
        {
            if (str.Length < Length)
                return "";
            else
                return str.Substring(0, Length);
        }

        static public string Right(string str, int Length)
        {
            if (str.Length < Length)
                return "";
            else
                return str.Substring(str.Length - Length);
        }

        static public string MySubString(string str, char flag1, char flag2)
        {

            int f1 = str.IndexOf(flag1);
            int f2 = str.IndexOf(flag2, f1 + 1);
            return str.Substring(f1 + 1, f2 - f1 - 1);
        }

        static public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }
}
