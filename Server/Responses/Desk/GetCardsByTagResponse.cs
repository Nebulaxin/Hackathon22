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
    public class GetCardsByTagResponse : Response
    {
        private const string commandGet = "SELECT * FROM Cards WHERE (desk = :id) AND (tag = :tag)";

        private bool badRequest;
        private string tag;
        private long id;
        public GetCardsByTagResponse(JsonNode node) : base(node)
        {
            try
            {
                id = request["id"];
                tag = request["tag"];
            }
            catch
            {
                badRequest = true;
            }
        }

        public override async Task<string> Process()
        {
            if (badRequest) return Util.BadRequest;

            var com = Server.Cards.CreateCommand(commandGet);
            com.Parameters.AddWithValue("id", id);
            com.Parameters.AddWithValue("tag", tag);
            using var reader = await com.ExecuteReaderAsync();
            JsonArray tags = new JsonArray();
            while (await reader.ReadAsync())
            {
                var card = new JsonObject();
                card.Add("id", reader.GetInt64(0));
                card.Add("name", reader.GetString(4));
                card.Add("description", reader.GetString(5));
                card.Add("created", reader.GetInt64(6));
                card.Add("expires", reader.GetInt64(7));
                card.Add("status", reader.GetString(8));
            }

            var result = new JsonObject();
            result.Add("cards", tags);
            result.Add("now", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

            return result.OKResult();
        }

        /*
        { // array
            "server":
            { // tag
                
                "users":
                [
                    "user1",
                    "user2",
                    "user3",
                    "user4",
                ],
                "cards":
                [
                    {
                        "name":"task1",
                        "description":"",
                        "expires":"-1",
                        "now":"2084993434",
                    },
                    {
                        "name":"task2",
                        "description":"description2",
                        "expires":"209962940",
                        "now":"2084993434",
                    },
                ]
            }
        ]
        */
    }
}
