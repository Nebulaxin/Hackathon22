using System;
using System.Security.Cryptography;
using System.Text;
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
        public static bool ContainsBadSymbols(string s) => s.EmptyOrWhitespaces() || badSymbols.Match(s).Success;
        public static bool EmptyOrWhitespaces(this string s) => string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s);

        public static byte[] GetHash(string inputString)
        {
            using (var sha = SHA256.Create())
                return sha.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string HashString64(string inputString) => Convert.ToBase64String(GetHash(inputString));

        public static async Task<long> GetUserId(string token)
        {
            const string getUserCommand = "SELECT id FROM Users WHERE (token = :token)";
            var com = Server.Users.CreateCommand(getUserCommand);
            com.Parameters.AddWithValue("token", token);
            long id;
            using(var reader = await com.ExecuteReaderAsync())
                id = reader.GetInt64(0);
            return id;
        }

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
