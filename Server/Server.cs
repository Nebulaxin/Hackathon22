using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        public static Database Users { get; private set; }

        public Server()
        {
            using var sr = File.OpenText("Database/config");
            Users = new Database(sr.ReadLine());
            Logger.Log("Users database connection created");
        }

        public async Task RunAsync(string host)
        {
            await Users.OpenAsync();
            await Logger.LogAsync("Users database opened");

            var l = new HttpListener();
            l.Prefixes.Add(host);

            l.Start();
            while (l.IsListening)
            {
                var context = await l.GetContextAsync();
                var req = new Request(context.Request);
                using (context.Response)
                using (var sw = new StreamWriter(context.Response.OutputStream))
                {
                    var resp = req.CreateResponse(context.Response);
                    //await resp.Process();

                    await sw.WriteAsync("da");
                }
            }
        }

        public static async Task Main()
        {
            var server = new Server();
#if DEBUG
            await server.RunAsync("http://localhost:5050/");
#else
            await server.RunAsync(Console.ReadLine());
#endif
        }
    }//http://185.177.218.151:8080/
}
