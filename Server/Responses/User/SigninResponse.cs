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
                password = Util.HashString64(request["password"]);
            }
            catch
            {
                badRequest = true;
            }
        }

        public override async Task<string> Process()
        {
            if (badRequest) return Util.BadRequest;

            if (Util.ContainsBadSymbols(username)) return Util.BadSymbols;
            if (Util.ContainsBadSymbols(password)) return Util.BadSymbols;

            var com = Server.Users.CreateCommand(command);
            using var reader = await com.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (reader.GetString(0) != username) continue;

                if (password != reader.GetString(1))
                    return Util.CodeToJson(Util.Code.WrongPassword);
                var result = new JsonObject();
                result.Add("token", reader.GetString(2));
                return result.OKResult();

            }
            return Util.CodeToJson(Util.Code.UserDoesntExist);
        }
    }
}
