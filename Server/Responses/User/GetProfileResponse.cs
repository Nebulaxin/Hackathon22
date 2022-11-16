using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Text;
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
            using var reader = await com.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return Util.CodeToJson(Util.Code.UserDoesntExist);
            var username = reader.GetString(0);
            var name = reader.GetString(1);



            var result = new JsonObject();
            result.Add("username", username);
            result.Add("name", name);
            return result.OKResult();


        }
    }
}
