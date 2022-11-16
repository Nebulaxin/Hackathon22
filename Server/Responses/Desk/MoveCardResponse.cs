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
    public class MoveCardResponse : Response
    {
        private const string commandGet = "SELECT * FROM Cards WHERE (id = :id)",
                                commandReplace = "REPLACE INTO Cards (id, desk, tag, admin, name, description, created, expires, status) VALUES (:id, :desk, :tag, :admin, :name, :description, :created, :expires, :status)";

        private bool badRequest;
        private string status;
        private long id;
        public MoveCardResponse(JsonNode node) : base(node)
        {
            try
            {
                id = request["id"];
                status = request["status"];
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
            using var reader = await com.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return Util.BadRequest;

            var com2 = Server.Cards.CreateCommand(commandReplace);
            com2.Parameters.AddWithValue("id", id);
            com2.Parameters.AddWithValue("desk", reader.GetInt64(1));
            com2.Parameters.AddWithValue("tag", reader.GetString(2));
            com2.Parameters.AddWithValue("admin", reader.GetString(3));
            com2.Parameters.AddWithValue("name", reader.GetString(4));
            com2.Parameters.AddWithValue("description", reader.GetString(5));
            com2.Parameters.AddWithValue("created", reader.GetInt64(6));
            com2.Parameters.AddWithValue("expires", reader.GetInt64(7));
            com2.Parameters.AddWithValue("status", status);
            await com2.ExecuteNonQueryAsync();

            return Util.OK;
        }
    }
}
