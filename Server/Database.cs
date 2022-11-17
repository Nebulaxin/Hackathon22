using System.Data.SQLite;
using System.Threading.Tasks;

namespace Server
{
    public class Database
    {
        private SQLiteConnection connection;

        public Database(string basePath)
        {
            connection = new SQLiteConnection($"Data Source={basePath};Version=3; FailIfMissing=false");
        }

        public async Task OpenAsync() => await connection.OpenAsync();

        public SQLiteCommand CreateCommand(string q)
        {
            var command = connection.CreateCommand();
            command.CommandText = q;
            return command;
        }

        public async Task CloseAsync() => await connection.CloseAsync();

    }
}
