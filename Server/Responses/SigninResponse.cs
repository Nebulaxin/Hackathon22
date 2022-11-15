using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NiceJson;

namespace Server.Responses
{
    public class SigninResponse : Response
    {
        private const string command = "SELECT username, password, token FROM Users";

        private string username, password;

        private bool badRequest;

        public SigninResponse(JsonNode request) : base(request)
        {
            try
            {
                username = request["username"];
                password = Hash.HashString64(request["password"]);
            }
            catch
            {
                badRequest = true;
            }
        }

        public override async Task<string> Process()
        {
            if (badRequest) return JsonUtil.BadRequest;

            var com = Server.Users.CreateCommand(command);
            using var reader = await com.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (reader.GetString(0) == username)
                {
                    if (password != reader.GetString(1))
                        return JsonUtil.CodeToJson(JsonUtil.Code.WrongPassword);
                    var result = new JsonObject();
                    result.Add("token", reader.GetString(2));
                    return result.OKResult();
                }
            }
            return JsonUtil.CodeToJson(JsonUtil.Code.UserDontExist);
        }
    }
}
