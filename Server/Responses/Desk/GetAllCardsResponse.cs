using System;
using System.Threading.Tasks;
using NiceJson;

namespace Server.Responses
{
    public class GetAllCardsResponse : Response
    {
        private const string commandGet = "SELECT * FROM Cards WHERE (desk = :id)",
                            getDesk = "SELECT name, admin FROM Desks WHERE (id = :id)";

        private string token;
        private bool badRequest;
        private long id;
        
        public GetAllCardsResponse(JsonNode node) : base(node)
        {
            try
            {
                id = request["id"];
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

            var com = Server.Desks.CreateCommand(getDesk);
            com.Parameters.AddWithValue("id", id);
            using var reader = await com.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return Util.BadRequest;
            var deskName = reader.GetString(0);
            var isAdmin = reader.GetString(1) == token;

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
            result.Add("isAdmin", isAdmin);
            result.Add("now", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

            return result.OKResult();
        }
    }
}
