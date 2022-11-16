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
    public class DeleteCardResponse : Response
    {
        private const string command = "DELETE FROM Cards WHERE (id = :id)";

        private bool badRequest;
        private string adminToken;
        private long id;
        public DeleteCardResponse(JsonNode node) : base(node)
        {
            try
            {
                adminToken = request["token"];
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

            var com = Server.Cards.CreateCommand(command);
            com.Parameters.AddWithValue("id", id);
            com.Parameters.AddWithValue("admin", adminToken);
            await com.ExecuteNonQueryAsync();



            // var result = new JsonObject();
            // result.Add("cards", tags);
            // result.Add("now", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

            return Util.OK;

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
