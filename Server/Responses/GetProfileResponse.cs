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
        private const string command = "SELECT * FROM Users WHERE token = :token";
        private bool badRequest;
        private string token;
        public GetProfileResponse() : base(null)
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
            if (badRequest) return JsonUtil.BadRequest;

            var com = Server.Users.CreateCommand(command);
            com.Parameters.AddWithValue("token", token);
            var reader = await com.ExecuteReaderAsync();

            var name = reader["name"];

            var result = new JsonObject();
            result.Add("token", (string)reader["token"]);
            return result.OKResult();
        }
    }
}