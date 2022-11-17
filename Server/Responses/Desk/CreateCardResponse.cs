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
    public class CreateCardResponse : Response
    {
        private const string command = "INSERT INTO Cards (desk, tag, admin, name, description, created, expires, status) VALUES (:desk, :tag, :admin, :name, :description, :created, :expires, 'todo');SELECT id FROM Cards WHERE (rowid=last_insert_rowid())",
                            getAdmin = "SELECT * FROM Desks WHERE (id = :id)";

        private bool badRequest;
        private string adminToken, tag, name, description;
        private long id, expires;
        public CreateCardResponse(JsonNode node) : base(node)
        {
            try
            {
                adminToken = request["token"];
                id = request["id"];
                tag = request["tag"];
                expires = request["expires"];
                name = request["name"];
                description = request["description"];
            }
            catch
            {
                badRequest = true;
            }
        }

        public override async Task<string> Process()
        {
            if (badRequest) return Util.BadRequest;

            if (Util.ContainsBadSymbols(tag)) return Util.BadSymbols;
            if (Util.ContainsBadSymbols(name)) return Util.BadSymbols;
            if (name.EmptyOrWhitespaces()) return Util.BadSymbols;
            if (description.EmptyOrWhitespaces()) return Util.BadSymbols;


            var com = Server.Desks.CreateCommand(getAdmin);
            com.Parameters.AddWithValue("id", id);
            using var reader = await com.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return Util.BadRequest;
            if (reader.GetString(2) != adminToken) return Util.CodeToJson(Util.Code.AccessViolation);


            com = Server.Cards.CreateCommand(command);
            com.Parameters.AddWithValue("desk", id);
            com.Parameters.AddWithValue("tag", tag);
            com.Parameters.AddWithValue("admin", adminToken);
            com.Parameters.AddWithValue("name", name);
            com.Parameters.AddWithValue("description", description);

            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            com.Parameters.AddWithValue("created", now);
            com.Parameters.AddWithValue("now", now);
            com.Parameters.AddWithValue("expires", now + (long)TimeSpan.FromDays(expires).TotalMilliseconds);

            long cardId = (long)await com.ExecuteScalarAsync();

            var result = new JsonObject();
            result.Add("now", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            result.Add("id", cardId);

            return result.OKResult();

        }

        /*
        
        {
            
            "cards":
            [
                {
                    "id":3,
                    "tag":"aboba",
                    "name":"task1",
                    "description":"",
                    "created":2084993434,
                    "expires":-1,
                    "status":"todo"
                },
                {
                    "id":4,
                    "tag":"aboba",
                    "name":"task1",
                    "description":"",
                    "created":2084993434,
                    "expires":-1,
                    "status":"todo"
                },
            ]
        }
        
        */
    }
}
