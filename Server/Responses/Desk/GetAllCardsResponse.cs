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
    public class GetAllCardsResponse : Response
    {
        private const string commandGet = "SELECT * FROM Cards WHERE (desk = :id)",
                            getDesk = "SELECT * FROM Desks WHERE (id = :id)";

        private bool badRequest;
        private long id;
        public GetAllCardsResponse(JsonNode node) : base(node)
        {
            try
            {
                id = request["id"];
            }
            catch
            {
                badRequest = true;
            }
        }

        public override async Task<string> Process()
        {
            if (badRequest) return Util.BadRequest;

            var com = Server.Desks.CreateCommand(getDesk);
            com.Parameters.AddWithValue("id", id);
            using var reader = await com.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return Util.BadRequest;
            var deskName = reader.GetString(1);

            com = Server.Cards.CreateCommand(commandGet);
            com.Parameters.AddWithValue("id", id);
            using var reader2 = await com.ExecuteReaderAsync();

            JsonArray allCards = new JsonArray();
            while (await reader2.ReadAsync())
            {
                var card = new JsonObject();
                card.Add("id", reader2.GetInt64(0));
                card.Add("tag", reader2.GetString(2));
                card.Add("name", reader2.GetString(4));
                card.Add("description", reader2.GetString(5));
                card.Add("created", reader2.GetInt64(6));
                card.Add("expires", reader2.GetInt64(7));
                card.Add("status", reader2.GetString(8));
                allCards.Add(card);
            }

            var result = new JsonObject();
            result.Add("cards", allCards);
            result.Add("name", deskName);
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
                        "id":0,
                        "name":"task1",
                        "description":"",
                        "expires":"-1",
                        "now":"2084993434",
                    },
                    {
                        "id":1,
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
