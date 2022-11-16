using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NiceJson;

namespace Server
{
    public static class Util
    {
        public static string OK => "{\"status\":0}";
        public static string BadRequest => "{\"status\":1}";
        public static string BadSymbols => "{\"status\":7}";

        public static string CodeToJson(Code c)
        {
            return $"{{\"status\":{(int)c}}}";
        }

        public static string CodeResult(this JsonObject obj, Code c)
        {
            obj.Add("status", (int)c);
            return obj.ToJsonString();
        }

        public static string OKResult(this JsonObject obj) => obj.CodeResult(Code.OK);

        private static Regex badSymbols = new Regex("/[^\\(a-zA-Z0-9_\\)]/m");
        public static bool ContainsBadSymbols(string s) => badSymbols.Match(s).Success;

        public enum Code
        {
            OK = 0,
            BadRequest = 1,
            UserAlreadyExists = 2,
            UserDoesntExist = 3,
            WrongPassword = 4,
            AccessViolation = 5,
            TagDoesntExist = 6,
            BadSymbols = 7,
            TooManyDesks = 8,
        }
    }
}
