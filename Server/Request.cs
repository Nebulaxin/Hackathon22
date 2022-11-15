using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Server.Responses;

namespace Server
{
    public static class Request
    {
        public static async Task<Response> CreateResponse(HttpListenerRequest req, HttpListenerResponse resp)
        {
            if (req.HttpMethod != HttpMethod.Post.Method)
            {
                Logger.Log($"Wrong method", Logger.Level.Error);
                return null;
            }

            if (req.ContentType != "application/json")
            {
                Logger.Log($"Non-JSON content", Logger.Level.Error);
                return null;
            }

            var reqString = req.Url.AbsolutePath[1..];
            if (!Enum.TryParse<RequestType>(reqString, true, out var requestType))
            {
                Logger.Log($"Unknown request: {reqString}", Logger.Level.Error);
                return null;
            }

            using var sw = new StreamReader(req.InputStream);
            var reqText = await sw.ReadToEndAsync();
            await Logger.LogAsync(reqText);
            var json = NiceJson.JsonNode.ParseJsonString(reqText);
            return requestType switch
            {
                RequestType.SignUp => new SignupResponse(json),
                RequestType.SignIn => new SigninResponse(json),
                RequestType.Test => new TestResponse(),
                _ => null
            };
        }

        public enum RequestType
        {
            SignUp,
            SignIn,
            Test
        }
    }
}
