using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NiceJson;

namespace Server
{
    public static class JsonUtil
    {
        public static string OK => "{\"status\":0}";
        public static string BadRequest => "{\"status\":1}";

        public static string CodeToJson(Code c)
        {
            return $"{{\"status\":{(int)c}}}";
        }

        public static JsonObject AddCode(this JsonObject obj, Code c)
        {
            obj.Add("status", (int)c);
            return obj;
        }

        public static string OKResult(this JsonObject obj) => obj.AddCode(Code.OK).ToJsonString();

        public enum Code
        {
            OK = 0,
            BadRequest = 1,
            UserAlreadyExists = 2,
            UserDontExist = 3,
            WrongPassword = 4,
        }
    }
}
