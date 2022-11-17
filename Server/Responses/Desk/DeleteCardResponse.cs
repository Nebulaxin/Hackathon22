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

            return Util.OK;

        }
    }
}
