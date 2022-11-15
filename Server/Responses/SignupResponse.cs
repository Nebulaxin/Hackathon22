using System;
using System.Threading.Tasks;
using NiceJson;

namespace Server.Responses
{
    public class SignupResponse : Response
    {
        private const string command = "INSERT INTO Users (username, password, name, token) VALUES (:username, :password, :name, :token)",
                            existsCommand = "SELECT COUNT(*) FROM Users WHERE (username = :username)";

        private string username, password, name;
        private bool badRequest;

        public SignupResponse(JsonNode request) : base(request)
        {
            try
            {
                username = request["username"];
                password = Hash.HashString64(request["password"]);
                name = request["name"];
            }
            catch
            {
                badRequest = true;
            }
        }

        public override async Task<string> Process()
        {
            if (badRequest) return JsonUtil.BadRequest;

            var com = Server.Users.CreateCommand(existsCommand);
            com.Parameters.AddWithValue(":username", username);

            if ((long)(await com.ExecuteScalarAsync()) > 0)
                return JsonUtil.CodeToJson(JsonUtil.Code.UserAlreadyExists);

            com = Server.Users.CreateCommand(command);
            com.Parameters.AddWithValue("username", username);

            com.Parameters.AddWithValue("password", password);
            com.Parameters.AddWithValue("name", name);

            var bytes = new byte[16];
            Server.Random.NextBytes(bytes);
            var token = Convert.ToBase64String(bytes);
            token = token.Replace('/', '-').Replace('+', '_').Replace('=', '%');
            com.Parameters.AddWithValue("token", token);

            await com.ExecuteNonQueryAsync();

            var result = new JsonObject();
            result.Add("token", token);
            return result.OKResult();
        }
    }
}
