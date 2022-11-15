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
        private const string command = "SELECT password FROM Users WHERE username = :username;",
                            existsCommand = "SELECT COUNT(*) FROM Users WHERE (username = :username)";

        private string username, password, token;

        private bool badRequest;

        public override bool CanGiveToken => true;

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

        public override string GetToken() => token;

        public override async Task<string> Process()
        {
            if (badRequest) return JsonUtil.BadRequest;

            var com = Server.Users.CreateCommand(command);
            com.Parameters.AddWithValue("username", username);
            var reader = await com.ExecuteReaderAsync();

            if (reader.FieldCount == 0)
                return JsonUtil.CodeToJson(JsonUtil.Code.UserDontExist);

            if ((string)reader["password"] != Hash.HashString64(password))
                return JsonUtil.CodeToJson(JsonUtil.Code.WrongPassword);

            token = (string)reader["token"];
            return JsonUtil.OK;
        }
    }
}
