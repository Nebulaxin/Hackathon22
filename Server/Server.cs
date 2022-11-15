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
                try
                {

                    var context = await l.GetContextAsync();
                    using (var sw = new StreamWriter(context.Response.OutputStream))
                    {
                        if (context.Request.HttpMethod != HttpMethod.Options.Method)
                        {
                            var resp = await Request.CreateResponse(context.Request, context.Response);

                            var answer = resp == null ? JsonUtil.BadRequest : await resp.Process();
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
            Random = new Random();
            var server = new Server();
#if DEBUG
            await server.RunAsync("http://localhost:5050/");
#else
            await server.RunAsync("http://185.177.218.151:5050/");
#endif
        }
    }//http://185.177.218.151:8080/
}
