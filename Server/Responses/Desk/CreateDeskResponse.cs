using System.Threading.Tasks;
using NiceJson;

namespace Server.Responses
{
    public class CreateDeskResponse : Response
    {
        private const string command = "INSERT INTO Desks (name, admin, users) VALUES (:name, :admin, '[]');SELECT id FROM Desks WHERE (rowid=last_insert_rowid())";
        
        private bool badRequest;
        private string token, name;
        
        public CreateDeskResponse(JsonNode node) : base(node)
        {
            try
            {
                token = request["token"];
                name = request["name"];
            }
            catch
            {
                badRequest = true;
            }
        }

        public override async Task<string> Process()
        {
            if (badRequest) return Util.BadRequest;
            if(name.EmptyOrWhitespaces()) return Util.BadSymbols;

            if (Util.ContainsBadSymbols(name)) return Util.BadSymbols;

            var com = Server.Desks.CreateCommand(command);
            com.Parameters.AddWithValue("name", name);
            com.Parameters.AddWithValue("admin", token);
            long id = (long)await com.ExecuteScalarAsync();

            var result = new JsonObject();
            result.Add("id", id);
            return result.OKResult();
        }
    }
}
