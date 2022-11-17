using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        public static Database Users { get; private set; }
        public static Database Desks { get; private set; }
        public static Database Cards { get; private set; }
        public static Random Random { get; private set; }

        private bool mustExit;

        public Server(StreamReader config)
        {
            Users = new Database(config.ReadLine());
            Logger.Log("Users database connection created");
            Desks = new Database(config.ReadLine());
            Logger.Log("Desks database connection created");
            Cards = new Database(config.ReadLine());
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

            Console.CancelKeyPress += (o, e) =>
            {
                e.Cancel = mustExit = true;
                l.Abort();
            };

            l.Prefixes.Add(host);

            l.Start();
            //var encoding = new UTF8Encoding(false);
            while (!mustExit && l.IsListening)
            {
                try
                {
                    var context = await l.GetContextAsync();
                    if(mustExit) break;
                    using (var sw = new StreamWriter(context.Response.OutputStream, Encoding.UTF8))
                    {
                        if (context.Request.HttpMethod != HttpMethod.Options.Method)
                        {
                            var resp = await Request.CreateResponse(context.Request, context.Response);

                            var answer = resp == null ? Util.BadRequest : await resp.Process();

                            await sw.WriteAsync(answer);
                            await Logger.LogAsync(answer);
                            await Logger.LogAsync(DateTime.UtcNow);
                        }
                        //await Logger.LogAsync(context.Request.Headers["Origin"]);
                        context.Response.AddHeader("Access-Control-Allow-Origin", "*");
                        context.Response.AddHeader("Access-Control-Allow-Headers", "*");
                        context.Response.AddHeader("Access-Control-Expose-Headers", "*");
                        context.Response.AppendHeader("cache-control", "no-cache, max-age=3600");
                        context.Response.AddHeader("x-content-type-options", "text/plain; charset=utf-8");
                        context.Response.AddHeader("content-type", "text/plain; charset=utf-8");

                        context.Response.ContentEncoding = Encoding.UTF8;
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        //await Logger.LogAsync(context.Response.Headers["Access-Control-Allow-Origin"]);
                    }
                }
                catch (ObjectDisposedException) { }
                catch (Exception e)
                {
                    Logger.Log(e);
                }
            }

            await Users.CloseAsync();
            await Logger.LogAsync("Users database closed");
            await Desks.CloseAsync();
            await Logger.LogAsync("Desks database closed");
            await Cards.CloseAsync();
            await Logger.LogAsync("Cards database closed");
        }

        public static async Task Main()
        {
            Random = new Random();
            Server server;
            string host;
            using (var config = File.OpenText("Database/config"))
            {
                host = await config.ReadLineAsync();
                server = new Server(config);
            }
            await server.RunAsync(host);
        }
    }//bgccY6IsQnBTwY69d6hVEg,,
}
