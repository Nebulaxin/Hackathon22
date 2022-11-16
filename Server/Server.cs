using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        public static Database Users { get; private set; }
        public static Database Desks { get; private set; }
        public static Database Cards { get; private set; }
        public static Random Random { get; private set; }

        public Server()
        {
            using var sr = File.OpenText("Database/config");
            Users = new Database(sr.ReadLine());
            Logger.Log("Users database connection created");
            Desks = new Database(sr.ReadLine());
            Logger.Log("Desks database connection created");
            Cards = new Database(sr.ReadLine());
            Logger.Log("Cards database connection created");
        }

        public async Task RunAsync(string host)
        {
            await Users.OpenAsync();
            await Logger.LogAsync("Users database opened");
            await Desks.OpenAsync();
            await Logger.LogAsync("Desks database opened");
            await Cards.OpenAsync();
            await Logger.LogAsync("Cards database opened");

            var l = new HttpListener();
            l.Prefixes.Add(host);

            l.Start();
            while (l.IsListening)
            {
                try
                {
                    var context = await l.GetContextAsync();
                    using (var sw = new StreamWriter(context.Response.OutputStream))
                    {
                        if (context.Request.HttpMethod != HttpMethod.Options.Method)
                        {
                            var resp = await Request.CreateResponse(context.Request, context.Response);

                            var answer = resp == null ? Util.BadRequest : await resp.Process();
                            await sw.WriteAsync(answer);
                            await Logger.LogAsync(answer);
                        }
                        context.Response.AddHeader("Access-Control-Allow-Origin", "*");
                        context.Response.AddHeader("Access-Control-Allow-Headers", "*");
                        context.Response.AppendHeader("cache-control", "no-cache, max-age=3600");
                        context.Response.AddHeader("x-content-type-options", "text/plain");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                }
            }
        }

        public static async Task Main()
        {
            Console.CancelKeyPress += (o, e) =>
            {
                Func<Task> close = async () =>
                {
                    await Users.CloseAsync();
                    await Logger.LogAsync("Users database closed");
                    await Desks.CloseAsync();
                    await Logger.LogAsync("Desks database closed");
                    await Cards.CloseAsync();
                    await Logger.LogAsync("Cards database closed");
                };
                Task.Run(close);
            };
            Random = new Random();
            var server = new Server();
#if DEBUG
            await server.RunAsync("http://localhost:5050/");
#else
            await server.RunAsync("http://185.177.218.151:5050/");
#endif
        }
    }//bgccY6IsQnBTwY69d6hVEg,,
}
