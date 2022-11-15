using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        public static Database Users { get; private set; }
        public static Random Random { get; private set; }

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
                using (context.Response)
                using (var sw = new StreamWriter(context.Response.OutputStream))
                {
                    var resp = await Request.CreateResponse(context.Request, context.Response);

                    if (resp != null && resp.CanRecieveToken)
                        resp.SetToken(context.Request.Cookies["token"].Value);

                    await sw.WriteAsync(resp == null ? JsonUtil.BadRequest : await resp.Process());

                    if (resp != null && resp.CanGiveToken)
                        context.Response.Cookies.Add(new Cookie("token", resp.GetToken()));

                    context.Response.AddHeader("Access-Control-Allow-Origin", "*");
                    context.Response.AddHeader("Access-Control-Allow-Headers", "X-Requested-With");

                }
            }
        }

        public static async Task Main()
        {
            Random = new Random();
            var server = new Server();
#if DEBUG
            await server.RunAsync("http://localhost:5050/");
#else
            await server.RunAsync(Console.ReadLine());
#endif
        }
    }//http://185.177.218.151:8080/
}
