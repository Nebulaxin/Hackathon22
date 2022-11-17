using System.Threading.Tasks;
using NiceJson;

namespace Server.Responses
{
    public class GetProfileResponse : Response
    {
        private const string getUsersCommand = "SELECT username, name, token FROM Users WHERE (token = :token)",
                                getDesksCommand = "SELECT id, name FROM Desks WHERE (admin = :admin)";
        
        private bool badRequest;
        private string token;

        public GetProfileResponse(JsonNode node) : base(node)
        {
            try
            {
                token = request["token"];
            }
            catch
            {
                badRequest = true;
            }
        }

        public override async Task<string> Process()
        {
            if (badRequest) return Util.BadRequest;

            var com = Server.Users.CreateCommand(getUsersCommand);
            com.Parameters.AddWithValue("token", token);
            string username, name;
            using (var reader = await com.ExecuteReaderAsync())
            {
                if (!await reader.ReadAsync()) return Util.CodeToJson(Util.Code.UserDoesntExist);
                username = reader.GetString(0);
                name = reader.GetString(1);
            }

            JsonArray desks = new JsonArray();
            com = Server.Desks.CreateCommand(getDesksCommand);
            com.Parameters.AddWithValue("admin", token);
            using (var reader = await com.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var desk = new JsonObject();
                    desk.Add("id", reader.GetInt64(0));
                    desk.Add("name", reader.GetString(1));
                    desks.Add(desk);
                }
            }

            var result = new JsonObject();
            result.Add("username", username);
            result.Add("name", name);
            result.Add("desks", desks);
            return result.OKResult();
        }
    }
}
